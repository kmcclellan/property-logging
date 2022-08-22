namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

using System.Buffers;
using System.Text;
using System.Text.Json;

[ProviderAlias("File")]
class FileLoggerProvider : ILoggerProvider
{
    readonly IEnumerable<ConfigureLogCollector<Utf8JsonWriter>> configureJson;
    readonly object sync = new();
    readonly IDisposable reload;
    readonly ObjectPool<JsonCollectorWriter> writers;

    Stream output;

    public FileLoggerProvider(
        IHostEnvironment env,
        IOptionsMonitor<FileLoggingOptions> fileOptions,
        IEnumerable<ConfigureLogCollector<Utf8JsonWriter>> configureJson,
        ObjectPoolProvider pools)
    {
        this.configureJson = configureJson;
        this.output = GetOutput(fileOptions.CurrentValue);
        this.reload = fileOptions.OnChange(
            options =>
            {
                lock (this.sync)
                {
                    this.output.Dispose();
                    this.output = GetOutput(options);
                }
            });

        Stream GetOutput(FileLoggingOptions options)
        {
            var path = Path.Combine(
                env.ContentRootPath,
                string.Format(options.Path, env.ApplicationName, env.EnvironmentName));

            return File.Open(path, options.Stream);
        }

        this.writers = pools.Create<JsonCollectorWriter>();
    }

    public ILogger CreateLogger(string category)
    {
        var onEntry = default(Action<Utf8JsonWriter, string, LogLevel, EventId>);
        var onMessage = default(Action<Utf8JsonWriter, string>);
        var onException = default(Action<Utf8JsonWriter, Exception>);
        var onProperty = default(Action<Utf8JsonWriter, string, object?>);

        foreach (var configure in this.configureJson)
        {
            if (LogCategory.Matches(category, configure.Category))
            {
                onEntry += configure.OnEntry;
                onMessage += configure.OnMessage;
                onException += configure.OnException;
                onProperty += configure.OnProperty;
            }
        }

        if (onEntry == null && onMessage == null && onException == null && onProperty == null)
        {
            return NullLogger.Instance;
        }

        return new JsonCollector(this, category, onEntry, onMessage, onException, onProperty).AsLogger();
    }

    public void Dispose()
    {
        this.reload.Dispose();
        this.output.Dispose();
    }

    void Write(ReadOnlySpan<byte> data)
    {
        // Support multithreaded logging.
        lock (this.sync)
        {
            this.output.Write(data);
        }
    }

    class JsonCollectorWriter
    {
        public JsonCollectorWriter()
        {
            this.Value = new(this.Buffer);
        }

        public ArrayBufferWriter<byte> Buffer { get; } = new();

        public Utf8JsonWriter Value { get; }
    }

    class JsonCollector : ILogCollector
    {
        readonly FileLoggerProvider provider;
        readonly string category;
        readonly Action<Utf8JsonWriter, string, LogLevel, EventId>? onEntry;
        readonly Action<Utf8JsonWriter, string>? onMessage;
        readonly Action<Utf8JsonWriter, Exception>? onException;
        readonly Action<Utf8JsonWriter, string, object?>? onProperty;

        public JsonCollector(
            FileLoggerProvider provider,
            string category,
            Action<Utf8JsonWriter, string, LogLevel, EventId>? onEntry,
            Action<Utf8JsonWriter, string>? onMessage,
            Action<Utf8JsonWriter, Exception>? onException,
            Action<Utf8JsonWriter, string, object?>? onProperty)
        {
            this.provider = provider;
            this.category = category;
            this.onEntry = onEntry;
            this.onMessage = onMessage;
            this.onException = onException;
            this.onProperty = onProperty;
        }

        public bool IsEnabled(LogLevel level)
        {
            return true;
        }

        public ILogEntryCollector Begin(LogLevel level, EventId id)
        {
            return new JsonEntryCollector(this, level, id);
        }

        struct JsonEntryCollector : ILogEntryCollector
        {
            static readonly byte[] Delimiter = Encoding.ASCII.GetBytes(Environment.NewLine);

            readonly JsonCollector collector;
            readonly JsonCollectorWriter writer;

            public JsonEntryCollector(JsonCollector collector, LogLevel level, EventId id)
            {
                this.collector = collector;
                this.writer = collector.provider.writers.Get();

                this.collector.onEntry?.Invoke(this.writer.Value, this.collector.category, level, id);
            }

            public bool SkipMessage => this.collector.onMessage == null;

            public bool SkipProperties => this.collector.onProperty == null;

            public void AddException(Exception exception)
            {
                this.collector.onException?.Invoke(this.writer.Value, exception);
            }

            public void AddMessage(string message)
            {
                this.collector.onMessage?.Invoke(this.writer.Value, message);
            }

            public void AddProperty(string name, object? value)
            {
                this.collector.onProperty?.Invoke(this.writer.Value, name, value);
            }

            public void Dispose()
            {
                this.writer.Value.WriteEndObject();
                this.writer.Value.Flush();
                this.writer.Buffer.Write(Delimiter);

                this.collector.provider.Write(this.writer.Buffer.WrittenSpan);
                this.writer.Value.Reset();
                this.writer.Buffer.Clear();

                this.collector.provider.writers.Return(this.writer);
            }
        }
    }
}
