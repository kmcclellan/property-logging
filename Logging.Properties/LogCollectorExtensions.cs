namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;

public static class LogCollectorExtensions
{
    public static ILogger AsLogger(this ILogCollector factory)
    {
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));
        return new CollectorLogger(factory);
    }

    public static ILogEntryCollector Skip(this ILogCollector factory)
    {
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));
        return NullEntry.Instance;
    }

    class CollectorLogger : ILogger, ISupportExternalScope
    {
        readonly ILogCollector factory;
        IExternalScopeProvider? scopes;

        public CollectorLogger(ILogCollector factory)
        {
            this.factory = factory;
        }

        public void SetScopeProvider(IExternalScopeProvider scopes)
        {
            this.scopes = scopes;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullLogger.Instance.BeginScope(state);
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

    class NullEntry : ILogEntryCollector
    {
        public static NullEntry Instance = new();

        private NullEntry()
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
