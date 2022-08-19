namespace Microsoft.Extensions.Logging.Properties;

public abstract class PropertySerializingProvider : ILoggerProvider, ISupportExternalScope
{
    readonly Stream output;

    IExternalScopeProvider? scopes;

    public PropertySerializingProvider(Stream output)
    {
        this.output = output;
    }

    public virtual void SetScopeProvider(IExternalScopeProvider scopes)
    {
        this.scopes = scopes;
    }

    public virtual ILogger CreateLogger(string category)
    {
        return this.CreateSerializer(category).AsLogger(this.output, this.scopes);
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected abstract ILogPropertySerializer CreateSerializer(string category);

    protected virtual void Dispose(bool disposing)
    {
    }
}
