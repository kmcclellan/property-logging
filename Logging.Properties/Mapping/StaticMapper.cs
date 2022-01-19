namespace Microsoft.Extensions.Logging.Properties.Mapping;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class StaticMapper<TProvider> : ILogPropertyMapper<TProvider>
{
    readonly IOptions<StaticPropertyOptions<TProvider>> options;

    public StaticMapper(IOptions<StaticPropertyOptions<TProvider>> options)
    {
        this.options = options;
    }

    public IEnumerable<KeyValuePair<string, object?>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null) =>
        options.Value.Values;
}
