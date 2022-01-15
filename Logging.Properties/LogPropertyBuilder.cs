namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Properties.Mappers;

using System;

class LogPropertyBuilder<TProvider> : ILogPropertyBuilder
{
    readonly IServiceCollection services;

    public LogPropertyBuilder(IServiceCollection services)
    {
        this.services = services;
    }

    public ILogPropertyBuilder FromEntry(
        string? level = null,
        string? category = null,
        string? eventId = null,
        string? state = null,
        string? exception = null,
        string? message = null)
    {
        return this.AddMapper<LogEntryMapper<TProvider>, LogEntryMapperOptions<TProvider>>(
            opts =>
            {
                TryAdd(level, LogEntryMapperOptions.MappingType.Level);
                TryAdd(category, LogEntryMapperOptions.MappingType.Category);
                TryAdd(eventId, LogEntryMapperOptions.MappingType.EventId);
                TryAdd(state, LogEntryMapperOptions.MappingType.State);
                TryAdd(exception, LogEntryMapperOptions.MappingType.Exception);
                TryAdd(message, LogEntryMapperOptions.MappingType.Message);

                void TryAdd(string? name, LogEntryMapperOptions.MappingType type)
                {
                    if (name != null)
                    {
                        opts.Mappings.Add(new(name, type));
                    }
                }
            });
    }

    public ILogPropertyBuilder FromException(string name, Func<Exception, object?> map, bool inner = false) =>
        this.AddMapper<LogExceptionMapper<TProvider>, LogExceptionMapperOptions<TProvider>>(
            opts => opts.Mappings.Add(new(name, map, inner)));

    public ILogPropertyBuilder FromException<T>(string name, Func<T, object?> map, bool inner = false)
        where T : Exception =>
        this.FromException(name, x => x is T t ? map(t) : null, inner);

    public ILogPropertyBuilder FromValue(string name, object value) =>
        this.AddMapper<LogValueMapper<TProvider>, LogValueMapperOptions<TProvider>>(
            opts => opts.Mappings.Add(new(name, value)));

    public ILogPropertyBuilder FromValue(string name, Func<object?> map) =>
        this.AddMapper<LogValueMapper<TProvider>, LogValueMapperOptions<TProvider>>(
            opts => opts.Mappings.Add(new(name, map)));

    private ILogPropertyBuilder AddMapper<TMapper, TOptions>(Action<TOptions> options)
        where TMapper : class, ILogPropertyMapper
        where TOptions : class
    {
        this.services.TryAddSingleton<TMapper>();
        this.services.AddOptions<LogPropertyOptions<TProvider>>()
            .Configure<TMapper>((opts, mapper) => opts.Mappers.Add(mapper));

        this.services.Configure(options);

        return this;
    }
}
