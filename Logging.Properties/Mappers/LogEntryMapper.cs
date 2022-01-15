namespace Microsoft.Extensions.Logging.Properties.Mappers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class LogEntryMapper<TProvider> : ILogPropertyMapper, IDisposable
{
    readonly IDisposable reload;

    ILookup<LogEntryMapperOptions.MappingType, string> mappings;

    public LogEntryMapper(IOptionsMonitor<LogEntryMapperOptions<TProvider>> options)
    {
        this.mappings = GetMappings(options.CurrentValue);
        this.reload = options.OnChange(x => this.mappings = GetMappings(x));

        static ILookup<LogEntryMapperOptions.MappingType, string> GetMappings(LogEntryMapperOptions options)
        {
            return options.Mappings.ToLookup(x => x.Type, x => x.Name);
        }
    }

    public IEnumerable<KeyValuePair<string, object>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null)
    {
        foreach (var name in this.mappings[LogEntryMapperOptions.MappingType.Level])
        {
            yield return new(name, entry.LogLevel);
        }

        foreach (var name in this.mappings[LogEntryMapperOptions.MappingType.Category])
        {
            yield return new(name, entry.Category);
        }

        foreach (var name in this.mappings[LogEntryMapperOptions.MappingType.EventId])
        {
            yield return new(name, entry.LogLevel);
        }

        if (entry.State != null)
        {
            foreach (var name in this.mappings[LogEntryMapperOptions.MappingType.State])
            {
                yield return new(name, entry.State);
            }
        }

        if (entry.Exception != null)
        {
            foreach (var name in this.mappings[LogEntryMapperOptions.MappingType.Exception])
            {
                yield return new(name, entry.Exception);
            }
        }

        if (entry.Formatter != null && this.mappings.Contains(LogEntryMapperOptions.MappingType.Message))
        {
            var message = entry.Formatter(entry.State, entry.Exception);

            foreach (var name in this.mappings[LogEntryMapperOptions.MappingType.Message])
            {
                yield return new(name, message);
            }
        }
    }

    public void Dispose() => this.reload.Dispose();
}
