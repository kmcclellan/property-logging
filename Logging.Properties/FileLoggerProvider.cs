namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

using System;
using System.Buffers;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;

/// <summary>
/// Provides loggers for writing to a file.
/// </summary>
[ProviderAlias("File")]
public sealed class FileLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
{
    static readonly byte[] Delimiter = Encoding.UTF8.GetBytes(Environment.NewLine);

    static readonly FileStreamOptions WriteOptions = new()
    {
        Mode = FileMode.Append,
        Access = FileAccess.Write,
        Share = FileShare.Read,
        Options = FileOptions.Asynchronous,
        BufferSize = 0,
    };

    readonly IHostEnvironment env;
    readonly IOptions<FileLoggingOptions> options;
    readonly IEnumerable<IConfigureLogEntry<Utf8JsonWriter>> configureEntry;
    readonly IEnumerable<IConfigureLogMessage<Utf8JsonWriter>> configureMessage;
    readonly IEnumerable<IConfigureLogException<Utf8JsonWriter>> configureException;
    readonly IEnumerable<IConfigureLogProperty<Utf8JsonWriter>> configureProperty;

    readonly ScopeSwitch scopes = new();
    readonly BufferBlock<JsonEntry> queue = new();
    readonly ObjectPool<JsonEntry> entries;
    readonly Task completion;

    internal FileLoggerProvider(
        IHostEnvironment env,
        IOptions<FileLoggingOptions> options,
        ObjectPoolProvider pools,
        IEnumerable<IConfigureLogEntry<Utf8JsonWriter>> configureEntry,
        IEnumerable<IConfigureLogMessage<Utf8JsonWriter>> configureMessage,
        IEnumerable<IConfigureLogException<Utf8JsonWriter>> configureException,
        IEnumerable<IConfigureLogProperty<Utf8JsonWriter>> configureProperty)
    {
        this.env = env;
        this.options = options;
        this.configureEntry = configureEntry;
        this.configureMessage = configureMessage;
        this.configureException = configureException;
        this.configureProperty = configureProperty;

        this.scopes.Set(options.Value.IncludeScopes);
        this.entries = pools.Create(JsonEntry.Pooling);

        var flushTask = Task.Factory.StartNew(
            this.Flush,
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Default)
            .ContinueWith(
                (task, obj) => ((IDataflowBlock)obj!).Fault(task.Exception!.GetBaseException()),
                this.queue,
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

        this.completion = Task.Factory.ContinueWhenAll(
            new[] { this.queue.Completion, flushTask },
            t => t[0].IsFaulted ? t[0] : t[1],
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default)
            .Unwrap();
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.scopes.Provider = scopeProvider;
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        var collectEntry = default(Action<Utf8JsonWriter, LogLevel, EventId>);
        var collectMessage = default(Action<Utf8JsonWriter, string>);
        var collectException = default(Action<Utf8JsonWriter, Exception>);
        var collectProperty = default(Action<Utf8JsonWriter, string, object?>);

        foreach (var configure in this.configureEntry)
        {
            if (LogCategory.Matches(categoryName, configure.Category))
            {
                collectEntry += (x, y, z) => configure.Log(x, categoryName, y, z);
            }
        }

        foreach (var configure in this.configureMessage)
        {
            if (LogCategory.Matches(categoryName, configure.Category))
            {
                collectMessage += configure.Log;
            }
        }

        foreach (var configure in this.configureException)
        {
            if (LogCategory.Matches(categoryName, configure.Category))
            {
                collectException += configure.Log;
            }
        }

        foreach (var configure in this.configureProperty)
        {
            if (LogCategory.Matches(categoryName, configure.Category))
            {
                collectProperty += configure.Log;
            }
        }

        return new FileCollector(this, collectEntry, collectMessage, collectException, collectProperty).AsLogger();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        this.queue.Complete();
        await this.completion.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.queue.Complete();
        this.completion.Wait();
    }

    async Task Flush()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var timestamp = DateTime.UtcNow;

        while (!this.queue.Completion.IsCompleted || buffer.WrittenCount > 0)
        {
            var delay = DateTime.UtcNow - timestamp + this.options.Value.Interval;

            if (buffer.WrittenCount < this.options.Value.BufferSize && delay > TimeSpan.Zero)
            {
                if (this.queue.TryReceive(out var entry))
                {
                    buffer.Write(entry.Data);
                    this.entries.Return(entry);

                    buffer.Write(Delimiter);
                }
                else
                {
                    try
                    {
                        await this.queue.OutputAvailableAsync().WaitAsync(delay).ConfigureAwait(false);
                    }
                    catch (TimeoutException)
                    {
                    }
                }
            }
            else
            {
                timestamp = DateTime.UtcNow;

                if (buffer.WrittenCount > 0)
                {
                    var path = Path.Combine(
                        this.env.ContentRootPath,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            this.options.Value.Path,
                            this.env.ApplicationName,
                            this.env.EnvironmentName));

                    try
                    {
                        using var output = File.Open(path, WriteOptions);
                        await output.WriteAsync(buffer.WrittenMemory).ConfigureAwait(false);
                    }
                    catch (IOException exception)
                    {
                        await Console.Error.WriteLineAsync(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Error flushing logs ({0} KiB). {1}",
                                buffer.WrittenCount >> 10,
                                exception))
                            .ConfigureAwait(false);
                    }

                    buffer.Clear();
                }
            }
        }
    }

    class JsonEntry
    {
        public static IPooledObjectPolicy<JsonEntry> Pooling = new PoolingPolicy();

        readonly ArrayBufferWriter<byte> byteWriter = new();

        private JsonEntry()
        {
            this.JsonWriter = new(this.byteWriter);
        }

        public IBufferWriter<byte> ByteWriter => this.byteWriter;

        public Utf8JsonWriter JsonWriter { get; }

        public ReadOnlySpan<byte> Data => this.byteWriter.WrittenSpan;

        class PoolingPolicy : IPooledObjectPolicy<JsonEntry>
        {
            public JsonEntry Create()
            {
                return new();
            }

            public bool Return(JsonEntry obj)
            {
                obj.JsonWriter.Reset();
                obj.byteWriter.Clear();

                return true;
            }
        }
    }

    class FileCollector : ILogCollector<FileEntry>
    {
        readonly Action<Utf8JsonWriter, LogLevel, EventId>? collectEntry;

        public FileCollector(
            FileLoggerProvider provider,
            Action<Utf8JsonWriter, LogLevel, EventId>? collectEntry,
            Action<Utf8JsonWriter, string>? collectMessage,
            Action<Utf8JsonWriter, Exception>? collectException,
            Action<Utf8JsonWriter, string, object?>? collectProperty)
        {
            this.Provider = provider;
            this.collectEntry = collectEntry;
            this.CollectMessage = collectMessage;
            this.CollectException = collectException;
            this.CollectProperty = collectProperty;
        }

        public FileLoggerProvider Provider { get; }

        public Action<Utf8JsonWriter, string>? CollectMessage { get; }

        public Action<Utf8JsonWriter, Exception>? CollectException { get; }

        public Action<Utf8JsonWriter, string, object?>? CollectProperty { get; }

        public bool IsEnabled(LogLevel level)
        {
            return this.collectEntry != null ||
                this.CollectMessage != null ||
                this.CollectException != null ||
                this.CollectProperty != null;
        }

        public FileEntry Begin(LogLevel level, EventId id)
        {
            var state = this.Provider.entries.Get();
            this.collectEntry?.Invoke(state.JsonWriter, level, id);
            return new(this, state);
        }
    }

    class FileEntry : ILogCollectorEntry
    {
        readonly FileCollector collector;
        readonly JsonEntry entry;

        public FileEntry(FileCollector collector, JsonEntry entry)
        {
            this.collector = collector;
            this.entry = entry;
        }

        public bool SkipMessage => this.collector.CollectMessage == null;

        public bool SkipProperties => this.collector.CollectProperty == null;

        public void AddMessage(string message)
        {
            this.collector.CollectMessage?.Invoke(this.entry.JsonWriter, message);
        }

        public void AddException(Exception exception)
        {
            this.collector.CollectException?.Invoke(this.entry.JsonWriter, exception);
        }

        public void AddProperty(string name, object? value)
        {
            this.collector.CollectProperty?.Invoke(this.entry.JsonWriter, name, value);
        }

        public void Dispose()
        {
            if (!this.collector.Provider.queue.Post(this.entry))
            {
                throw this.collector.Provider.queue.Completion.Exception?.GetBaseException() ??
                    throw new ObjectDisposedException(null);
            }
        }
    }
}

