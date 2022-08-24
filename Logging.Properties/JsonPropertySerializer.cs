namespace Microsoft.Extensions.Logging.Properties;

using System.Buffers;
using System.Text.Json;

class JsonPropertySerializer : IPropertySerializer<Utf8JsonWriter>
{
    readonly IPropertySerializer<IBufferWriter<byte>> serializer;

    public JsonPropertySerializer(IPropertySerializer<IBufferWriter<byte>> serializer)
    {
        this.serializer = serializer;
    }

    public IDisposable Begin(out Utf8JsonWriter writer)
    {
        var entry = this.serializer.Begin(out var buffer);
        writer = new(buffer);
        writer.WriteStartObject();

        return new JsonEntry(entry, writer);
    }

    struct JsonEntry : IDisposable
    {
        readonly IDisposable entry;
        readonly Utf8JsonWriter writer;

        public JsonEntry(IDisposable entry, Utf8JsonWriter writer)
        {
            this.entry = entry;
            this.writer = writer;
        }

        public void Dispose()
        {
            this.writer.WriteEndObject();
            this.writer.Dispose();
            this.entry.Dispose();
        }
    }
}
