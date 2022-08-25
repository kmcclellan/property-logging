namespace Microsoft.Extensions.Logging.Properties;

using System.Collections.Concurrent;

/// <summary>
/// A base implementation for logger providers using property logging components.
/// </summary>
public class PropertyLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    readonly IPropertyCollectorFactory collectors;
    readonly ConcurrentDictionary<string, ILogger> loggers = new();

    IExternalScopeProvider? scopes;

    /// <summary>
    /// Initializes the provider.
    /// </summary>
    /// <param name="collectors">The collector factory for log properties.</param>
    public PropertyLoggerProvider(IPropertyCollectorFactory collectors)
    {
        this.collectors = collectors;
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.scopes = scopeProvider;

        foreach (var logger in this.loggers.Values.OfType<ISupportExternalScope>())
        {
            logger.SetScopeProvider(scopeProvider);
        }
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        if (!this.loggers.TryGetValue(categoryName, out var logger))
        {
            logger = this.collectors.Create(categoryName).AsLogger();

            if (this.scopes != null && logger is ISupportExternalScope scopeLogger)
            {
                scopeLogger.SetScopeProvider(this.scopes);
            }

            this.loggers[categoryName] = logger;
        }

        return logger;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes/releases resources used by the service.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to dispose managed resources,
    /// <see langword="false"/> to release unmanaged resources only.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
    }
}
