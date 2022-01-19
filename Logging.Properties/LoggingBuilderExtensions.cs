namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Properties.Mapping;
using Microsoft.Extensions.Options;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Property logging extensions to <see cref="ILoggingBuilder"/>.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds default implementations of <see cref="ILogPropertyMapper{TProvider}"/> to the logging services.
    /// </summary>
    /// <remarks>
    /// Configure mappers using property options, which bind to provider configuration as follows:
    /// <list type="bullet">
    /// <item><c>Properties:Entry</c> to <see cref="EntryPropertyOptions{TProvider}"/>.</item>
    /// <item><c>Properties:EventId</c> to <see cref="EventIdPropertyOptions{TProvider}"/>.</item>
    /// <item><c>Properties:Exception</c> to <see cref="ExceptionPropertyOptions{TProvider}"/>.</item>
    /// <item><c>Properties:Environment</c> to <see cref="EnvironmentPropertyOptions{TProvider}"/>.</item>
    /// <item><c>Properties:Timestamp</c> to <see cref="TimestampPropertyOptions{TProvider}"/>.</item>
    /// <item><c>Properties:Static</c> to <see cref="StaticPropertyOptions{TProvider}"/>.</item>
    /// <item><c>Properties:State</c> to <see cref="StatePropertyOptions{TProvider}"/>.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TProvider">The associated logger provider type.</typeparam>
    /// <param name="builder">The logging builder.</param>
    /// <returns>The same instance, for chaining.</returns>
    public static ILoggingBuilder AddProperties<TProvider>(this ILoggingBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AddConfiguration();
        builder.Services
            .AddMapper<TProvider, EntryMapper<TProvider>, EntryPropertyOptions<TProvider>>("Entry")
            .AddMapper<TProvider, EventIdMapper<TProvider>, EventIdPropertyOptions<TProvider>>("EventId")
            .AddMapper<TProvider, ExceptionMapper<TProvider>, ExceptionPropertyOptions<TProvider>>("Exception")
            .AddMapper<TProvider, EnvironmentMapper<TProvider>, EnvironmentPropertyOptions<TProvider>>("Environment")
            .AddMapper<TProvider, TimestampMapper<TProvider>, TimestampPropertyOptions<TProvider>>("Timestamp")
            .AddMapper<TProvider, StaticMapper<TProvider>, StaticPropertyOptions<TProvider>>("Static")
            .AddMapper<TProvider, StateMapper<TProvider>, StatePropertyOptions<TProvider>>("State");

        return builder;
    }

    private static IServiceCollection AddMapper<TProvider, TMapper, TOptions>(
        this IServiceCollection services,
        string sectionName)
        where TMapper : class, ILogPropertyMapper<TProvider>
        where TOptions : class
    {
        services.TryAddSingleton(new PropertyOptionsSection<TOptions>(sectionName));
        services.TryAddEnumerable(
            new[]
            {
                ServiceDescriptor.Singleton<ILogPropertyMapper<TProvider>, TMapper>(),

                ServiceDescriptor.Singleton<
                    IConfigureOptions<TOptions>,
                    ConfigurePropertyOptions<TProvider, TOptions>>(),

                ServiceDescriptor.Singleton<
                    IOptionsChangeTokenSource<TOptions>,
                    ConfigurePropertyOptions<TProvider, TOptions>>()
            });

        return services;
    }

    private class PropertyOptionsSection<TOptions>
    {
        public PropertyOptionsSection(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }

    [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
    private class ConfigurePropertyOptions<TProvider, TOptions> :
        ConfigurationChangeTokenSource<TOptions>, IConfigureOptions<TOptions>
        where TOptions : class
    {
        readonly IConfiguration config;

        public ConfigurePropertyOptions(
            ILoggerProviderConfiguration<TProvider> providerConfig,
            PropertyOptionsSection<TOptions> section)
            : this(providerConfig.Configuration.GetSection(ConfigurationPath.Combine("Properties", section.Name)))
        {
        }

        private ConfigurePropertyOptions(IConfiguration config) : base(config)
        {
            this.config = config;
        }

        public void Configure(TOptions options) => this.config.Bind(options);
    }
}
