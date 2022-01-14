namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class LogPropertyMapper<TProvider> : ILogPropertyMapper<TProvider>
{
    readonly IEnumerable<ILogProperty> properties;

    public LogPropertyMapper(IEnumerable<ILogProperty> properties)
    {
        this.properties = properties;
    }

    public IEnumerable<KeyValuePair<string, object>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null)
    {
        foreach (var property in properties)
        {
            if (property.ProviderType == typeof(TProvider))
            {
                foreach (var value in property.GetValues(entry, scopes))
                {
                    yield return new(property.Name, value);
                }
            }
        }
    }
}
