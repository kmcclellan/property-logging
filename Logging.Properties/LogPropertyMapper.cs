namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class LogPropertyMapper<TProvider> : ILogPropertyMapper<TProvider>
{
    readonly IOptions<LogPropertyOptions<TProvider>> options;

    public LogPropertyMapper(IOptions<LogPropertyOptions<TProvider>> options)
    {
        this.options = options;
    }

    public IEnumerable<KeyValuePair<string, object>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null)
    {
        foreach (var mapper in options.Value.Mappers)
        {
            foreach (var kvp in mapper.Map(entry, scopes))
            {
                yield return kvp;
            }
        }
    }
}
