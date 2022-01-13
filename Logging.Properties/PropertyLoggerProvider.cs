namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;

using System;

/// <summary>
/// A base provider for logging named properties.
/// </summary>
public abstract class PropertyLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    readonly ILogPropertyMapper mapper;

    /// <summary>
    /// Initializes the provider.
    /// </summary>
    /// <param name="mapper">The log property mapper.</param>
    protected PropertyLoggerProvider(ILogPropertyMapper mapper)
    {
        this.mapper = mapper;
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => this.mapper.SetScopes(scopeProvider);

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName) => new PropertyLogger(this, categoryName);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Writes properties for a log entry.
    /// </summary>
    /// <param name="properties">The log properties.</param>
    protected abstract void Log(IEnumerable<KeyValuePair<string, object>> properties);

    /// <summary>
    /// Disposes and/or finalizes the instance.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to include managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
    }

    private void Log<TState>(LogEntry<TState> entry)
    {
        var properties = this.mapper.Map(entry);
        this.Log(properties);
    }

    private class PropertyLogger : ILogger
    {
        readonly PropertyLoggerProvider provider;
        readonly string category;

        public PropertyLogger(PropertyLoggerProvider provider, string category)
        {
            this.provider = provider;
            this.category = category;
        }

        public IDisposable BeginScope<TState>(TState state) => throw new NotSupportedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            this.provider.Log<TState>(new(logLevel, this.category, eventId, state, exception, formatter));
    }
}
