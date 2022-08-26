namespace Microsoft.Extensions.Logging.Properties;

class ScopeSwitch
{
    readonly SwitchedScopeProvider provider = new();

    public IExternalScopeProvider Provider
    {
        get => this.provider;
        set => this.provider.Set(value);
    }

    public void Set(bool enabled)
    {
        this.provider.Set(enabled);
    }

    class SwitchedScopeProvider : IExternalScopeProvider
    {
        bool enabled;
        IExternalScopeProvider? provider;

        public void Set(bool enabled)
        {
            this.enabled = enabled;
        }

        public void Set(IExternalScopeProvider provider)
        {
            this.provider = provider;
        }

        public void ForEachScope<TState>(Action<object?, TState> callback, TState state)
        {
            if (this.enabled)
            {
                this.provider?.ForEachScope(callback, state);
            }
        }

        public IDisposable Push(object? state)
        {
            throw new NotSupportedException();
        }
    }
}
