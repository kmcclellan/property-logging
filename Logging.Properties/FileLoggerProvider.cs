namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

using System.Buffers;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;

/// <summary>
/// Provides loggers for writing to a file.
/// </summary>
[ProviderAlias("File")]
public sealed class FileLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
{
    readonly IOptions<FileLoggingOptions> options;
    readonly IEnumerable<IConfigureLogEntry<Utf8JsonWriter>> configureEntry;
    readonly IEnumerable<IConfigureLogMessage<Utf8JsonWriter>> configureMessage;
    readonly IEnumerable<IConfigureLogException<Utf8JsonWriter>> configureException;
    readonly IEnumerable<IConfigureLogProperty<Utf8JsonWriter>> configureProperty;

    readonly ObjectPool<JsonEntry> entries;
    readonly ITargetBlock<JsonEntry> target;

    IExternalScopeProvider? scopes;

    internal FileLoggerProvider(
        IHostEnvironment env,
        IOptions<FileLoggingOptions> options,
        ObjectPoolProvider pools,
        IEnumerable<IConfigureLogEntry<Utf8JsonWriter>> configureEntry,
        IEnumerable<IConfigureLogMessage<Utf8JsonWriter>> configureMessage,
        IEnumerable<IConfigureLogException<Utf8JsonWriter>> configureException,
        IEnumerable<IConfigureLogProperty<Utf8JsonWriter>> configureProperty)
    {
        this.options = options;
        this.configureEntry = configureEntry;
        this.configureMessage = configureMessage;
        this.configureException = configureException;
        this.configureProperty = configureProperty;

        this.entries = pools.Create(JsonEntry.Pooling);

        var path = Path.Combine(
                env.ContentRootPath,
                string.Format(
                    CultureInfo.InvariantCulture,
                    options.Value.Path,
                    env.ApplicationName,
                    env.EnvironmentName));

        this.target = new LogFileTarget<JsonEntry>(
            path,
            (writer, entry) =>
            {
                writer.Write(entry.Data);
                this.entries.Return(entry);
            },
            options.Value.BufferSize,
            options.Value.FlushInterval);
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        if (this.options.Value.IncludeScopes)
        {
            this.scopes = scopeProvider;
        }
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

        return new FileCollector(this, collectEntry, collectMessage, collectException, collectProperty)
            .AsLogger(this.scopes);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        this.target.Complete();
        await this.target.Completion.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.target.Complete();
        this.target.Completion.Wait();
    }

    class JsonEntry
    {
        public static readonly IPooledObjectPolicy<JsonEntry> Pooling = new PoolingPolicy();

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

    readonly struct FileEntry : ILogCollectorEntry
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
            if (!this.collector.Provider.target.Post(this.entry))
            {
                throw this.collector.Provider.target.Completion.Exception?.GetBaseException() ??
                    throw new ObjectDisposedException(null);
            }
        }
    }
}

