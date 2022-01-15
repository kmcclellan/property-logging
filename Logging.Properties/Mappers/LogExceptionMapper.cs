namespace Microsoft.Extensions.Logging.Properties.Mappers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class LogExceptionMapper<TProvider> : ILogPropertyMapper
{
    readonly IDisposable reload;

    ILookup<bool, (string, Func<Exception, object?>)> mappings;

    public LogExceptionMapper(IOptionsMonitor<LogExceptionMapperOptions<TProvider>> options)
    {
        this.mappings = GetMappings(options.CurrentValue);
        this.reload = options.OnChange(x => this.mappings = GetMappings(x));

        static ILookup<bool, (string, Func<Exception, object?>)> GetMappings(LogExceptionMapperOptions options)
        {
            return options.Mappings.ToLookup(x => x.IsRecursive, x => (x.Name, x.Map));
        }
    }

    public IEnumerable<KeyValuePair<string, object>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null)
    {
        if (entry.Exception != null)
        {
            foreach (var kvp in this.Map(entry.Exception, false))
            {
                yield return kvp;
            }

            if (this.mappings.Contains(true))
            {
                foreach (var exception in Flatten(entry.Exception))
                {
                    foreach (var kvp in this.Map(exception, true))
                    {
                        yield return kvp;
                    }
                }
            }
        }    
    }

    public void Dispose() => this.reload.Dispose();

    private IEnumerable<KeyValuePair<string, object>> Map(Exception exception, bool key)
    {
        foreach (var (name, map) in this.mappings[key])
        {
            var value = map(exception);

            if (value != null)
            {
                yield return new(name, value);
            }
        }
    }

    private static IEnumerable<Exception> Flatten(Exception exception)
    {
        if (exception is AggregateException agg)
        {
            foreach (var inner1 in agg.InnerExceptions)
            {
                foreach (var inner2 in Flatten(inner1))
                {
                    yield return inner2;
                }
            }
        }
        else
        {
            yield return exception;

            if (exception.InnerException != null)
            {
                foreach (var inner in Flatten(exception))
                {
                    yield return inner;
                }
            }
        }
    }
}
