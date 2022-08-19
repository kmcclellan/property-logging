namespace Microsoft.Extensions.Logging.Properties;

public interface ILogCollector
{
    bool IsEnabled(LogLevel level);

    ILogEntryCollector Begin(LogLevel level, EventId id);
}
