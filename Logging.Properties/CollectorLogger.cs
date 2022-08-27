namespace Microsoft.Extensions.Logging.Properties;

class CollectorLogger<T> : ILogger
{
    readonly ILogCollector<T> collector;
    readonly IExternalScopeProvider? scopes;
    readonly Action<object?, T> collectProperties;

    public CollectorLogger(ILogCollector<T> collector, IExternalScopeProvider? scopes)
    {
        this.collector = collector;
        this.scopes = scopes;

        this.collectProperties = (object? state, T entry) =>
        {
            if (state is IEnumerable<KeyValuePair<string, object?>> properties)
            {
                foreach (var (key, value) in properties)
                {
                    collector.AddProperty(entry, key, value);
                }
            }
        };
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
        var entry = this.collector.Begin(level, eventId, out var skipMessage, out var skipProperties);

        if (exception != null)
        {
            collector.AddException(entry, exception);
        }

        if (!skipMessage)
        {
            this.collector.AddMessage(entry, formatter(state, exception));
        }

        if (!skipProperties)
        {
            this.scopes?.ForEachScope(this.collectProperties, entry);
            this.collectProperties(state, entry);
        }

        this.collector.Finish(entry);
    }
}
