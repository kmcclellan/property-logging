namespace Microsoft.Extensions.Logging.Properties;

public interface IPropertyCollector<out TEntry>
    where TEntry : IPropertyEntry
{
    bool IsEnabled(LogLevel level);

    TEntry Begin(LogLevel level, EventId id);
}

public interface IPropertyCollector : IPropertyCollector<IPropertyEntry>
{
}
