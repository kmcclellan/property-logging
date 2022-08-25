namespace Microsoft.Extensions.Logging.Properties.Json;

using Microsoft.Extensions.DependencyInjection;

using System.Buffers;
using System.Text.Json;

public static class LogPropertyBuilderExtensions
{
    public static LogPropertyBuilder<TProvider, Utf8JsonWriter> AsJson<TProvider>(
        this LogPropertyBuilder<TProvider, IBufferWriter<byte>> builder,
        JsonLogType logType = JsonLogType.Dictionary)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        builder.Services.Configure<JsonPropertyOptions<TProvider>>(x => x.LogType = logType);

        return new LogPropertyBuilder<TProvider>(builder.Services)
            .Serialize<JsonPropertySerializer<TProvider>, Utf8JsonWriter>();
    }
}
