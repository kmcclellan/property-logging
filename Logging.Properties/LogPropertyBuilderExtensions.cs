namespace Microsoft.Extensions.Logging.Properties;

using System.Buffers;
using System.Text.Json;

public static class LogPropertyBuilderExtensions
{
    public static LogPropertyBuilder<TProvider, Utf8JsonWriter> AsJson<TProvider>(
        this LogPropertyBuilder<TProvider, IBufferWriter<byte>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        return new LogPropertyBuilder<TProvider>(builder.Services)
            .Serialize<JsonLogSerializer, Utf8JsonWriter>();
    }
}
