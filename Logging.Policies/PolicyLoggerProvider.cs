namespace Microsoft.Extensions.Logging.Policies;

/// <summary>
/// Creates loggers using <see cref="ILoggingPolicy{TEntry}"/>.
/// </summary>
/// <typeparam name="TEntry">The logging policy entry type.</typeparam>
public abstract class PolicyLoggerProvider<TEntry> : ILoggerProvider, ISupportExternalScope
{
    readonly bool includeScopes;
    IExternalScopeProvider? scopes;

    /// <summary>
    /// Initializes the provider.
    /// </summary>
    /// <param name="includeScopes">Whether to include scopes when writing logs.</param>
    protected PolicyLoggerProvider(bool includeScopes = false)
    {
        this.includeScopes = includeScopes;
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        if (this.includeScopes)
        {
            this.scopes = scopeProvider;
        }
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes and/or finalizes the instance.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to dispose and finalize, <see langword="false"/> to finalize only.</param>
    protected virtual void Dispose(bool disposing)
    {
    }
}
