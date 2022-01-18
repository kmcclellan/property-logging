namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;

using System;

/// <summary>
/// A base provider for logging named properties.
/// </summary>
public abstract class PropertyLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    readonly IEnumerable<ILogPropertyMapper> mappers;

    IExternalScopeProvider? scopes;

    /// <summary>
    /// Initializes the provider with the given property mappers.
    /// </summary>
    /// <param name="mappers">The log property mappers.</param>
    protected PropertyLoggerProvider(IEnumerable<ILogPropertyMapper> mappers)
    {
        this.mappers = mappers;
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => this.scopes = scopeProvider;

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
    protected abstract void Log(IEnumerable<KeyValuePair<string, object?>> properties);

    /// <summary>
    /// Disposes and/or finalizes the instance.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to include managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
    }

    private void Log<TState>(LogEntry<TState> entry) => this.Log(this.Map(entry));

    private IEnumerable<KeyValuePair<string, object?>> Map<TState>(LogEntry<TState> entry)
    {
        foreach (var mapper in this.mappers)
        {
            foreach (var property in mapper.Map(entry, this.scopes))
            {
                yield return property;
            }
        }
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
