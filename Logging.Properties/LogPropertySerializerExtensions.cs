namespace Microsoft.Extensions.Logging.Properties;

using System.Buffers;
using System.Collections.Generic;

public static class LogPropertySerializerExtensions
{
    public static ILogger AsLogger(
        this ILogPropertySerializer serializer,
        Stream output,
        IExternalScopeProvider? scopes = null)
    {
        ArgumentNullException.ThrowIfNull(serializer, nameof(serializer));
        return new SerializingCollector(serializer, output).AsLogger(scopes);
    }

    private class SerializingCollector : ILogPropertyCollector
    {
        readonly ILogPropertySerializer serializer;
        readonly Stream output;

        public SerializingCollector(ILogPropertySerializer collector, Stream output)
        {
            this.serializer = collector;
            this.output = output;
        }

        public bool IsEnabled(LogLevel level)
        {
            return this.serializer.IsEnabled(level);
        }

        public ILogPropertyCollection Begin(LogLevel level, EventId eventId, Exception? exception)
        {
            if (this.IsEnabled(level))
            {
                var writer = new ArrayBufferWriter<byte>();
                this.serializer.Begin(writer, level, eventId);

                if (exception != null)
                {
                    this.serializer.WriteException(writer, exception);
                }

                return new SerializingCollection(this.serializer, writer, this.output);
            }

            return this.Skip();
        }
    }

    private struct SerializingCollection : ILogPropertyCollection
    {
        readonly ILogPropertySerializer serializer;
        readonly ArrayBufferWriter<byte> writer;
        readonly Stream output;

        public SerializingCollection(
            ILogPropertySerializer serializer,
            ArrayBufferWriter<byte> writer,
            Stream output)
        {
            this.serializer = serializer;
            this.writer = writer;
            this.output = output;
        }

        public bool SkipMessage => false;

        public bool SkipProperties => false;

        public void AddMessage(string message)
        {
            this.serializer.WriteMessage(this.writer, message);
        }

        public void AddProperty(KeyValuePair<string, object?> kvp)
        {
            this.serializer.WriteProperty(this.writer, kvp.Key, kvp.Value);
        }

        public void Dispose()
        {
            this.serializer.Finish(this.writer);
            this.output.Write(this.writer.WrittenSpan);
        }
    }
}
