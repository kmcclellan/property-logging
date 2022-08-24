namespace Microsoft.Extensions.Logging.Properties;

using System.Buffers;
using System.Text.Json;

class JsonLogSerializer : ILogSerializer<Utf8JsonWriter>
{
    readonly ILogSerializer<IBufferWriter<byte>> serializer;

    public JsonLogSerializer(ILogSerializer<IBufferWriter<byte>> serializer)
    {
        this.serializer = serializer;
    }

    public IDisposable Begin(out Utf8JsonWriter writer)
    {
        var entry = new JsonLogEntry(this.serializer);
        writer = entry.Writer;
        return entry;
    }

    struct JsonLogEntry : IDisposable
    {
        readonly IDisposable entry;
        readonly IBufferWriter<byte> buffer;

        public JsonLogEntry(ILogSerializer<IBufferWriter<byte>> serializer)
        {
            this.entry = serializer.Begin(out this.buffer);

            this.Writer = new(buffer);
            this.Writer.WriteStartObject();
        }

        public Utf8JsonWriter Writer { get; }

        public void Dispose()
        {
            this.Writer.WriteEndObject();
            this.Writer.Dispose();

            this.entry.Dispose();
        }
    }
}
