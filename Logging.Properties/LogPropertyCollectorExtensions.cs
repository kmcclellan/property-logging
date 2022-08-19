namespace Microsoft.Extensions.Logging.Properties;

public static class LogPropertyCollectorExtensions
{
    public static ILogger AsLogger(this ILogPropertyCollector collector, IExternalScopeProvider? scopes = null)
    {
        ArgumentNullException.ThrowIfNull(collector, nameof(collector));
        return new PropertyLogger(collector, scopes);
    }

    public static ILogPropertyCollection Skip(this ILogPropertyCollector collector)
    {
        ArgumentNullException.ThrowIfNull(collector, nameof(collector));
        return NullCollection.Instance;
    }

    private class PropertyLogger : ILogger
    {
        readonly ILogPropertyCollector collector;
        readonly IExternalScopeProvider? scopes;

        public PropertyLogger(ILogPropertyCollector collector, IExternalScopeProvider? scopes)
        {
            this.collector = collector;
            this.scopes = scopes;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException();
        }

        public bool IsEnabled(LogLevel level)
        {
            return this.collector.IsEnabled(level);
        }

        public void Log<TState>(
            LogLevel level,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            using var collection = collector.Begin(level, eventId, exception);

            if (!collection.SkipMessage)
            {
                collection.AddMessage(formatter(state, exception));
            }

            if (!collection.SkipProperties)
            {
                this.scopes?.ForEachScope((x, y) => Collect(x, y), collection);
                Collect(state, collection);
            }
        }

        static void Collect(object? state, ILogPropertyCollection collection)
        {
            if (state is IEnumerable<KeyValuePair<string, object?>> properties)
            {
                foreach (var property in properties)
                {
                    collection.AddProperty(property);
                }
            }
        }
    }

    private class NullCollection : ILogPropertyCollection
    {
        public static ILogPropertyCollection Instance = new NullCollection();

        private NullCollection()
        {
        }

        public bool SkipMessage => true;

        public bool SkipProperties => true;

        public void AddMessage(string message)
        {
            throw new NotSupportedException();
        }

        public void AddProperty(KeyValuePair<string, object?> kvp)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
        }
    }
}
