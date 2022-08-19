namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging;

public interface ILogPropertyCollector
{
    bool IsEnabled(LogLevel level);

    ILogPropertyCollection Begin(LogLevel level, EventId eventId, Exception? exception);
}
