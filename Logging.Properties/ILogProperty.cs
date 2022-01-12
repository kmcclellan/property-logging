namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;

interface ILogProperty
{
    string Name { get; }

    Type ProviderType { get; }

    IEnumerable<object> GetValues<TState>(LogEntry<TState> entry, IExternalScopeProvider? scopes);
}
