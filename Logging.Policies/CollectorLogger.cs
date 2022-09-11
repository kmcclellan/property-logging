namespace Microsoft.Extensions.Logging.Policies;

class CollectorLogger<TEntry> : ILogger
    where TEntry : ILogEntryCollector
{
    readonly ILogCollector<TEntry> collector;

    public CollectorLogger(ILogCollector<TEntry> collector)
    {
        this.collector = collector;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
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
            Collect(state, entry);
        }
    }

    static void Collect<T>(T state, ILogEntryCollector entry)
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
