namespace Microsoft.Extensions.Logging.Properties;

public interface ILogCollectorFactory
{
    bool IsEnabled(LogLevel level);

    ILogCollector Create(LogLevel level, EventId id);
}
