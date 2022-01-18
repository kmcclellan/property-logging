namespace Microsoft.Extensions.Logging.Properties.Mapping;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using static EntryPropertyOptions;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class EntryMapper<TProvider> : ILogPropertyMapper<TProvider>
{
    readonly IOptions<EntryPropertyOptions<TProvider>> options;

    public EntryMapper(IOptions<EntryPropertyOptions<TProvider>> options)
    {
        this.options = options;
    }

    public IEnumerable<KeyValuePair<string, object?>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null)
    {
        foreach (var mapping in this.options.Value.Mappings)
        {
            object? value = mapping.Key switch
            {
                EntryField.LogLevel => entry.LogLevel,
                EntryField.Category => entry.Category,
                EntryField.EventId => entry.EventId != 0 ? entry.EventId : null,
                EntryField.State => entry.State,
                EntryField.Exception => entry.Exception,
                EntryField.Message => entry.Formatter?.Invoke(entry.State, entry.Exception),
                _ => null,
            };

            if (value != null)
            {
                yield return new(mapping.Value, value);
            }
        }
    }
}
