namespace Microsoft.Extensions.Logging.Properties;

class CollectorLogger<TEntry> : ILogger
    where TEntry : ILogCollectorEntry
{
    readonly ILogCollector<TEntry> collector;
    readonly IExternalScopeProvider? scopes;

    public CollectorLogger(ILogCollector<TEntry> collector, IExternalScopeProvider? scopes)
    {
        this.collector = collector;
        this.scopes = scopes;
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
        using var entry = this.collector.Begin(level, eventId);

        if (exception != null)
        {
            entry.AddException(exception);
        }

        if (!entry.SkipMessage)
        {
            entry.AddMessage(formatter(state, exception));
        }

        if (!entry.SkipProperties)
        {
            this.scopes?.ForEachScope((x, y) => Collect(x, y), entry);
            Collect(state, entry);
        }
    }

    static void Collect(object? state, ILogCollectorEntry entry)
    {
        if (state is IEnumerable<KeyValuePair<string, object?>> properties)
        {
            foreach (var (key, value) in properties)
            {
                entry.AddProperty(key, value);
            }
        }
    }
}
