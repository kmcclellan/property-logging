namespace Microsoft.Extensions.Logging.Properties;

public static class LogCollectorExtensions
{
    public static ILogger AsLogger(this ILogCollector factory, IExternalScopeProvider? scopes = null)
    {
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));
        return new CollectorLogger(factory, scopes);
    }

    public static ILogEntryCollector Skip(this ILogCollector factory)
    {
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));
        return NullCollector.Instance;
    }

    class CollectorLogger : ILogger
    {
        readonly ILogCollector factory;
        readonly IExternalScopeProvider? scopes;

        public CollectorLogger(ILogCollector factory, IExternalScopeProvider? scopes)
        {
            this.factory = factory;
            this.scopes = scopes;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException();
        }

        public bool IsEnabled(LogLevel level)
        {
            return this.factory.IsEnabled(level);
        }

        public void Log<TState>(
            LogLevel level,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            using var collector = factory.Begin(level, eventId);

            if (exception != null)
            {
                collector.AddException(exception);
            }

            if (!collector.SkipMessage)
            {
                collector.AddMessage(formatter(state, exception));
            }

            if (!collector.SkipProperties)
            {
                this.scopes?.ForEachScope((x, y) => Collect(x, y), collector);
                Collect(state, collector);
            }
        }

        static void Collect(object? state, ILogEntryCollector collector)
        {
            if (state is IEnumerable<KeyValuePair<string, object?>> properties)
            {
                foreach (var (key, value) in properties)
                {
                    collector.AddProperty(key, value);
                }
            }
        }
    }

    class NullCollector : ILogEntryCollector
    {
        public static ILogEntryCollector Instance = new NullCollector();

        private NullCollector()
        {
        }

        public bool SkipMessage => true;

        public bool SkipProperties => true;

        public void AddMessage(string message)
        {
        }

        public void AddException(Exception exception)
        {
        }

        public void AddProperty(string name, object? value)
        {
        }

        public void Dispose()
        {
        }
    }
}
