namespace Microsoft.Extensions.Logging.Properties;

public static class LogPropertyCollectorExtensions
{
    public static ILogEntryCollector Map(this ILogPropertyCollector collector, LogPropertyMappings mappings)
    {
        ArgumentNullException.ThrowIfNull(collector, nameof(collector));
        ArgumentNullException.ThrowIfNull(mappings, nameof(mappings));

        return new MappedCollector(collector, mappings);
    }

    private class MappedCollector : ILogEntryCollector
    {
        readonly ILogPropertyCollector collector;
        readonly LogPropertyMappings mappings;

        public MappedCollector(ILogPropertyCollector collector, LogPropertyMappings mappings)
        {
            this.collector = collector;
            this.mappings = mappings;
        }

        public bool SkipMessage => throw new NotImplementedException();

        public bool SkipProperties => throw new NotImplementedException();

        public void AddMessage(string message)
        {
            throw new NotImplementedException();
        }

        public void AddException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void AddProperty(string name, object? value)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
