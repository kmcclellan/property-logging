namespace Microsoft.Extensions.Logging.Properties.Mappers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class LogValueMapper<TProvider> : ILogPropertyMapper
{
    readonly IOptions<LogValueMapperOptions<TProvider>> options;

    public LogValueMapper(IOptions<LogValueMapperOptions<TProvider>> options)
    {
        this.options = options;
    }

    public IEnumerable<KeyValuePair<string, object>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null)
    {
        foreach (var mapping in this.options.Value.Mappings)
        {
            if (mapping.Value != null)
            {
                yield return new(mapping.Name, mapping.Value);
            }

            if (mapping.Map != null)
            {
                var value = mapping.Map();

                if (value != null)
                {
                    yield return new(mapping.Name, value);
                }
            }
        }
    }
}
