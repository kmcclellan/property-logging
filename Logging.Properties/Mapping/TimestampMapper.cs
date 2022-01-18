namespace Microsoft.Extensions.Logging.Properties.Mapping;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class TimestampMapper<TProvider> : ILogPropertyMapper<TProvider>
{
    readonly IOptions<TimestampPropertyOptions<TProvider>> options;

    public TimestampMapper(IOptions<TimestampPropertyOptions<TProvider>> options)
    {
        this.options = options;
    }

    public IEnumerable<KeyValuePair<string, object>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null)
    {
        var mapping = options.Value.Mapping;

        if (mapping != null)
        {
            yield return new(mapping, DateTimeOffset.Now);
        }
    }
}
