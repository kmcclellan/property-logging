namespace Microsoft.Extensions.Logging.Properties;

public abstract class PropertyCollectingProvider : ILoggerProvider, ISupportExternalScope
{
    IExternalScopeProvider? scopes;

    public virtual void SetScopeProvider(IExternalScopeProvider scopes)
    {
        this.scopes = scopes;
    }

    public virtual ILogger CreateLogger(string category)
    {
        return this.CreateCollector(category).AsLogger(this.scopes);
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected abstract ILogPropertyCollector CreateCollector(string category);

    protected virtual void Dispose(bool disposing)
    {
    }
}
