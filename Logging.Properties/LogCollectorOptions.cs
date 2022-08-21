namespace Microsoft.Extensions.Logging.Properties;

public class LogCollectorOptions<TEntry>
{
    public string Category { get; set; } = "*";

    public Action<TEntry, string, LogLevel, EventId>? OnEntry { get; set; }

    public Action<TEntry, string>? OnMessage { get; set; }

    public Action<TEntry, Exception>? OnException { get; set; }

    public Action<TEntry, string, object?> OnProperty { get; set; }
}
