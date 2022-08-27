namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// A base implementation of a logger provider using <see cref="ILogCollector{TEntry}"/>.
/// </summary>
/// <typeparam name="TEntry">The log collector entry type.</typeparam>
public abstract class CollectorLoggerProvider<TEntry> : ILoggerProvider, ISupportExternalScope
    where TEntry : ILogCollectorEntry
{
    readonly ScopeSwitch scopes = new();

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.scopes.Provider = scopeProvider;
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        return this.GetCollector(categoryName).AsLogger(this.scopes.Provider);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Enables or disables logging using <see cref="IExternalScopeProvider"/>.
    /// </summary>
    /// <param name="enable"><see langword="true"/> to enable, <see langword="false"/> to disable.</param>
    protected void ConfigureScopes(bool enable)
    {
        this.scopes.Set(enable);
    }

    /// <summary>
    /// Gets a collector to use for logging.
    /// </summary>
    /// <param name="category">The log category name.</param>
    /// <returns>The log collector.</returns>
    protected abstract ILogCollector<TEntry> GetCollector(string category);

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
