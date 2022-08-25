namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

public class LogPropertyBuilder<TProvider>
{
    public LogPropertyBuilder(IServiceCollection services)
    {
        this.Services = services;
    }

    public IServiceCollection Services { get; }

    public OptionsBuilder<PropertyLoggingOptions<TProvider>> Options { get; }

    public LogPropertyBuilder<TProvider, TWriter> Serialize<T, TWriter>()
        where T : class, IPropertySerializer<TWriter>
    {
        this.Services.TryAddSingleton<T>();
        this.Services.TryAddSingleton(
            provider =>
            {
                var serializer = provider.GetRequiredService<T>();

                return serializer is IPropertySerializer<TWriter, TProvider> wrapped
                    ? wrapped
                    : new SerializerWrapper<T, TWriter>(serializer);
            });

        this.Services.TryAddSingleton<
            IPropertyCollectorFactory<TProvider>,
            PropertyCollectorFactory<TWriter, TProvider>>();

        return new(this.Services);
    }

    class SerializerWrapper<T, TWriter> : IPropertySerializer<TWriter, TProvider>
        where T : class, IPropertySerializer<TWriter>
    {
        readonly T serializer;

        public SerializerWrapper(T serializer)
        {
            this.serializer = serializer;
        }

        public IDisposable Begin(out TWriter writer)
        {
            return this.serializer.Begin(out writer);
        }
    }
}

public class LogPropertyBuilder<TProvider, TWriter>
{
    public LogPropertyBuilder(IServiceCollection services)
    {
        this.Services = services;
        this.Collector = services.AddOptions<PropertyCollectorOptions<TWriter, TProvider>>();
    }

    public IServiceCollection Services { get; }

    public OptionsBuilder<PropertyCollectorOptions<TWriter, TProvider>> Collector { get; }

    public LogPropertyBuilder<TProvider, TWriter> OnEntry(
        Action<TWriter, string, LogLevel, EventId> action,
        string? category = null)
    {
        return this.AddAction(new() { Category = category, OnEntry = action });
    }

    public LogPropertyBuilder<TProvider, TWriter> OnMessage(
        Action<TWriter, string> action,
        string? category = null)
    {
        return this.AddAction(new() { Category = category, OnMessage = action });
    }

    public LogPropertyBuilder<TProvider, TWriter> OnException(
        Action<TWriter, Exception> action,
        string? category = null)
    {
        return this.AddAction(new() { Category = category, OnException = action });
    }

    public LogPropertyBuilder<TProvider, TWriter> OnProperty(
        Action<TWriter, string, object?> action,
        string? category = null)
    {
        return this.AddAction(new() { Category = category, OnProperty = action });
    }

    LogPropertyBuilder<TProvider, TWriter> AddAction(LogWriteAction<TWriter> action)
    {
        this.Collector.Configure(x => x.Actions.Add(action));
        return this;
    }
}
