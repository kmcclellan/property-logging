namespace Microsoft.Extensions.Logging.Properties.Json;

using System.Text.Json;

public class JsonPropertyOptions
{
    public JsonLogType LogType { get; set; } = JsonLogType.Dictionary;

    public JsonWriterOptions Writer { get; set; }
}

public class JsonPropertyOptions<TProvider> : JsonPropertyOptions
{
}
