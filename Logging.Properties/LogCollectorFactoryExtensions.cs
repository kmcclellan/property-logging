namespace Microsoft.Extensions.Logging.Properties;

public static class LogCollectorFactoryExtensions
{
    public static ILogger AsLogger(this ILogCollectorFactory factory, IExternalScopeProvider? scopes = null)
    {
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));
        return new CollectorLogger(factory, scopes);
    }

    public static ILogCollector Skip(this ILogCollectorFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));
        return NullCollector.Instance;
    }

    private class CollectorLogger : ILogger
    {
        readonly ILogCollectorFactory factory;
        readonly IExternalScopeProvider? scopes;

        public CollectorLogger(ILogCollectorFactory factory, IExternalScopeProvider? scopes)
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
            using var collector = factory.Create(level, eventId);

            if (exception != null)
            {
                collector.AddException(exception);
            }

            if (collector is ILogMessageCollector msg)
            {
                msg.AddMessage(formatter(state, exception));
            }

            if (collector is ILogPropertyCollector prop)
            {
                this.scopes?.ForEachScope((x, y) => Collect(x, y), prop);
                Collect(state, prop);
            }
        }

        static void Collect(object? state, ILogPropertyCollector collector)
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

    private class NullCollector : ILogCollector
    {
        public static ILogCollector Instance = new NullCollector();

        private NullCollector()
        {
        }

        public void AddException(Exception exception)
        {
        }

        public void Dispose()
        {
        }
    }
}
