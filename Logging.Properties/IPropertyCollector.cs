namespace Microsoft.Extensions.Logging.Properties;

public interface IPropertyCollector
{
    bool IsEnabled(LogLevel level);

    IPropertyEntry Begin(LogLevel level, EventId id);
}
