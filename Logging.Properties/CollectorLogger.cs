namespace Microsoft.Extensions.Logging.Properties;

class CollectorLogger<T> : ILogger
{
    readonly ILogCollector<T> collector;
    readonly IExternalScopeProvider? scopes;
    readonly ILogMessageCollector<T>? messageCollector;
    readonly Action<object?, T>? collectProperties;

    public CollectorLogger(ILogCollector<T> collector, IExternalScopeProvider? scopes)
    {
        this.collector = collector;
        this.scopes = scopes;

        this.messageCollector = collector as ILogMessageCollector<T>;

        if (collector is ILogPropertyCollector<T> propertyCollector)
        {
            this.collectProperties = (object? state, T entry) =>
            {
                if (state is IEnumerable<KeyValuePair<string, object?>> properties)
                {
                    foreach (var (key, value) in properties)
                    {
                        propertyCollector.AddProperty(entry, key, value);
                    }
                }
            };
        }
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotSupportedException($"Use '{nameof(IExternalScopeProvider)}' instead.");
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
        if (collector.Begin(level, eventId) is { } entry)
        {
            if (exception != null)
            {
                collector.AddException(entry, exception);
            }

            this.messageCollector?.AddMessage(entry, formatter(state, exception));

            if (this.collectProperties != null)
            {
                this.scopes?.ForEachScope(this.collectProperties, entry);
                this.collectProperties(state, entry);
            }

            this.collector.Finish(entry);
        }
    }
}
