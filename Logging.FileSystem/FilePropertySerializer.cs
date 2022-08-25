namespace Microsoft.Extensions.Logging.FileSystem;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Properties;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class FilePropertySerializer : IPropertySerializer<IBufferWriter<byte>, FileLoggerProvider>, IDisposable
{
    readonly IOptionsMonitor<FileLoggingOptions> options;
    readonly ObjectPool<ArrayBufferWriter<byte>> writers;
    readonly object sync = new();
    readonly IDisposable reload;

    Stream output;

    public FilePropertySerializer(IHostEnvironment env, IOptionsMonitor<FileLoggingOptions> options, ObjectPoolProvider pools)
    {
        this.options = options;
        this.writers = pools.Create<ArrayBufferWriter<byte>>();
        this.output = GetOutput(options.CurrentValue);
        this.reload = options.OnChange(
            opts =>
            {
                lock (this.sync)
                {
                    this.output?.Dispose();
                    this.output = GetOutput(opts);
                }
            });

        Stream GetOutput(FileLoggingOptions opts)
        {
            var path = Path.Combine(
                env.ContentRootPath,
                string.Format(CultureInfo.InvariantCulture, opts.Path, env.ApplicationName, env.EnvironmentName));

            return File.Open(
                path,
                new FileStreamOptions
                {
                    Mode = FileMode.Append,
                    Access = FileAccess.Write,
                    Share = FileShare.Read,
                    BufferSize = opts.BufferSize,
                });
        }
    }

    public IDisposable Begin(out IBufferWriter<byte> writer)
    {
        var arrayWriter = this.writers.Get();
        writer = arrayWriter;
        return new SerializerEntry(this, arrayWriter);
    }

    public void Dispose()
    {
        this.reload.Dispose();
        this.output?.Dispose();
    }

    struct SerializerEntry : IDisposable
    {
        readonly FilePropertySerializer serializer;
        readonly ArrayBufferWriter<byte> writer;

        public SerializerEntry(FilePropertySerializer serializer, ArrayBufferWriter<byte> writer)
        {
            this.serializer = serializer;
            this.writer = writer;
        }

        public void Dispose()
        {
            if (this.serializer.options.CurrentValue.Delimiter is { } delim)
            {
                this.writer.Write(delim);
            }

            // Support multi-threaded entries.
            lock (this.serializer.sync)
            {
                this.serializer.output.Write(this.writer.WrittenSpan);
            }

            this.writer.Clear();
            this.serializer.writers.Return(this.writer);
        }
    }
}
