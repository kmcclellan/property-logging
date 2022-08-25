namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Options;

using System.Collections.Concurrent;

/// <summary>
/// A base implementation for logger providers using property logging components.
/// </summary>
public class PropertyLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    readonly IOptionsMonitor<PropertyLoggingOptions> options;
    readonly IPropertyCollectorFactory collectors;
    readonly IDisposable reload;
    readonly ConcurrentDictionary<string, ILogger> loggers = new();

    IExternalScopeProvider scopes = NullScopes.Instance;

    /// <summary>
    /// Initializes the provider.
    /// </summary>
    /// <param name="options">The property logging options.</param>
    /// <param name="collectors">The collector factory for log properties.</param>
    public PropertyLoggerProvider(IOptionsMonitor<PropertyLoggingOptions> options, IPropertyCollectorFactory collectors)
    {
        this.options = options;
        this.collectors = collectors;
        this.reload = options.OnChange(x => this.SetLoggerScopes(x.IncludeScopes ? this.scopes : NullScopes.Instance));
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.scopes = scopeProvider;

        if (this.options.CurrentValue.IncludeScopes)
        {
            this.SetLoggerScopes(this.scopes);
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
        if (disposing)
        {
            this.reload.Dispose();
        }
    }

    void SetLoggerScopes(IExternalScopeProvider scopes)
    {
        foreach (var logger in this.loggers.Values.OfType<ISupportExternalScope>())
        {
            logger.SetScopeProvider(scopes);
        }
    }

    class NullScopes : IExternalScopeProvider
    {
        public static NullScopes Instance = new();

        private NullScopes()
        {
        }

        public IDisposable Push(object? state)
        {
            throw new NotSupportedException();
        }

        public void ForEachScope<TState>(Action<object?, TState> callback, TState state)
        {
        }
    }
}
