namespace Microsoft.Extensions.Logging.Properties.Json;

using System.Buffers;
using System.Text.Json;

public static class LogPropertyBuilderExtensions
{
    public static JsonPropertyBuilder<TProvider> AsJson<TProvider>(
        this LogPropertyBuilder<TProvider, IBufferWriter<byte>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        _ = new LogPropertyBuilder<TProvider>(builder.Services)
            .Serialize<JsonPropertySerializer<TProvider>, Utf8JsonWriter>();

        return new(builder.Services);
    }
}
