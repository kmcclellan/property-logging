namespace Microsoft.Extensions.Logging.Properties.Json;

using Microsoft.Extensions.Options;

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class JsonPropertySerializer<TProvider> : IPropertySerializer<Utf8JsonWriter, TProvider>
{
    readonly IOptions<JsonPropertyOptions> options;
    readonly IPropertySerializer<IBufferWriter<byte>> serializer;

    public JsonPropertySerializer(
        IOptions<JsonPropertyOptions<TProvider>> options,
        IPropertySerializer<IBufferWriter<byte>> serializer)
    {
        this.options = options;
        this.serializer = serializer;
    }

    public IDisposable Begin(out Utf8JsonWriter writer)
    {
        var opts = this.options.Value;
        var entry = this.serializer.Begin(out var buffer);
        writer = new(buffer, opts.Writer);

        switch (opts.LogType)
        {
            case JsonLogType.Dictionary:
                writer.WriteStartObject();
                break;

            case JsonLogType.Array:
                writer.WriteStartArray();
                break;
        }

        return new JsonEntry(opts, entry, writer);
    }

    struct JsonEntry : IDisposable
    {
        readonly JsonPropertyOptions options;
        readonly IDisposable entry;
        readonly Utf8JsonWriter writer;

        public JsonEntry(JsonPropertyOptions options, IDisposable entry, Utf8JsonWriter writer)
        {
            this.options = options;
            this.entry = entry;
            this.writer = writer;
        }

        public void Dispose()
        {
            switch (this.options.LogType)
            {
                case JsonLogType.Dictionary:
                    this.writer.WriteEndObject();
                    break;

                case JsonLogType.Array:
                    this.writer.WriteEndArray();
                    break;
            }

            this.writer.Dispose();
            this.entry.Dispose();
        }
    }
}
