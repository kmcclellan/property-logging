namespace Microsoft.Extensions.Logging.Properties.Mapping;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using static EventIdPropertyOptions;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class EventIdMapper<TProvider> : ILogPropertyMapper<TProvider>
{
    readonly IOptions<EventIdPropertyOptions<TProvider>> options;

    public EventIdMapper(IOptions<EventIdPropertyOptions<TProvider>> options)
    {
        this.options = options;
    }

    public IEnumerable<KeyValuePair<string, object?>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null)
    {
        if (entry.EventId != 0)
        {
            foreach (var mapping in this.options.Value.Mappings)
            {
                object? value = mapping.Key switch
                {
                    EventIdField.Id => entry.EventId.Id,
                    EventIdField.Name => entry.EventId.Name,
                    _ => null,
                };

                if (value != null)
                {
                    yield return new(mapping.Value, value);
                }
            }
        }
    }
}
