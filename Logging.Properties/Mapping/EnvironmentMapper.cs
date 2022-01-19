namespace Microsoft.Extensions.Logging.Properties.Mapping;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
#if !NET5_0_OR_GREATER
using System.Diagnostics;
#endif
using System.Diagnostics.CodeAnalysis;

using static Environment;
using static Microsoft.Extensions.Logging.Properties.Mapping.EnvironmentPropertyOptions;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class EnvironmentMapper<TProvider> : ILogPropertyMapper<TProvider>
{
#if !NET5_0_OR_GREATER
    static int processId;
#endif

    readonly IOptions<EnvironmentPropertyOptions<TProvider>> options;

    public EnvironmentMapper(IOptions<EnvironmentPropertyOptions<TProvider>> options)
    {
        this.options = options;
    }

#if !NET5_0_OR_GREATER
    static int ProcessId
    {
        get
        {
            if (processId == 0)
            {
                using var process = Process.GetCurrentProcess();
                processId = process.Id;
            }

            return processId;
        }
    }
#endif

    public IEnumerable<KeyValuePair<string, object?>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null)
    {
        foreach (var mapping in this.options.Value.Mappings)
        {
            object? value = mapping.Key switch
            {
                EnvironmentField.MachineName => MachineName,
                EnvironmentField.UserName => UserName,
                EnvironmentField.Version => Version,
                EnvironmentField.OSVersion => OSVersion,
                EnvironmentField.ProcessId => ProcessId,
                _ => null,
            };

            if (value != null)
            {
                yield return new(mapping.Value, value);
            }
        }
    }
}
