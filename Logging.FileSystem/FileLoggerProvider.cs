namespace Microsoft.Extensions.Logging.FileSystem;

using Microsoft.Extensions.Logging.Properties;

using System.Collections.Concurrent;

/// <summary>
/// Provides loggers for writing to a file.
/// </summary>
[ProviderAlias("File")]
public sealed class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    readonly IPropertyCollectorFactory<FileLoggerProvider> collectors;
    readonly ConcurrentDictionary<string, ILogger> loggers = new();

    IExternalScopeProvider? scopes;

    internal FileLoggerProvider(IPropertyCollectorFactory<FileLoggerProvider> collectors)
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
    }
}
