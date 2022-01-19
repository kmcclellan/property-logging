namespace Microsoft.Extensions.Logging.Properties.Mapping;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using static Microsoft.Extensions.Logging.Properties.Mapping.ExceptionPropertyOptions;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class ExceptionMapper<TProvider> : ILogPropertyMapper<TProvider>
{
    readonly IOptions<ExceptionPropertyOptions<TProvider>> options;

    public ExceptionMapper(IOptions<ExceptionPropertyOptions<TProvider>> options)
    {
        this.options = options;
    }

    public IEnumerable<KeyValuePair<string, object?>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null) =>
        Map(entry.Exception, this.options.Value);

    private static IEnumerable<KeyValuePair<string, object?>> Map(Exception? exception, ExceptionPropertyOptions options)
    {
        if (options.IsRecursive && exception is AggregateException aggregate)
        {
            foreach (var inner in aggregate.InnerExceptions)
            {
                foreach (var property in Map(inner, options))
                {
                    yield return property;
                }
            }
        }
        else if (exception != null)
        {
            foreach (var mapping in options.Mappings)
            {
                object? value = mapping.Key switch
                {
                    ExceptionField.Type => exception.GetType(),
                    ExceptionField.Message => exception.Message,
                    ExceptionField.StackTrace => exception.StackTrace,
                    ExceptionField.Source => exception.Source,
                    ExceptionField.TargetSite => exception.TargetSite,
                    ExceptionField.HResult => exception.HResult,
                    ExceptionField.HelpLink => exception.HelpLink,
                    _ => null,
                };

                if (value != null)
                {
                    yield return new(mapping.Value, value);
                }
            }

            if (options.IsRecursive)
            {
                foreach (var property in Map(exception.InnerException, options))
                {
                    yield return property;
                }
            }
        }
    }
}
