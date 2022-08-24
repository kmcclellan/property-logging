namespace Microsoft.Extensions.Logging.Properties;

public class LogWriteAction<TWriter>
{
    public string? Category { get; set; }

    public Action<TWriter, string, LogLevel, EventId>? OnEntry { get; set; }

    public Action<TWriter, string>? OnMessage { get; set; }

    public Action<TWriter, Exception>? OnException { get; set; }

    public Action<TWriter, string, object?>? OnProperty { get; set; }
}
