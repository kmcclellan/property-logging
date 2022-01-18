namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Properties.Mapping;

/// <summary>
/// Property logging extensions to <see cref="ILoggingBuilder"/>.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds default implementations of <see cref="ILogPropertyMapper{TProvider}"/> to the logging services.
    /// </summary>
    /// <remarks>
    /// Configure mappers using property options:
    /// <list type="bullet">
    /// <item><see cref="EntryPropertyOptions{TProvider}"/>.</item>
    /// <item><see cref="EventIdPropertyOptions{TProvider}"/>.</item>
    /// <item><see cref="ExceptionPropertyOptions{TProvider}"/>.</item>
    /// <item><see cref="EnvironmentPropertyOptions{TProvider}"/>.</item>
    /// <item><see cref="TimestampPropertyOptions{TProvider}"/>.</item>
    /// <item><see cref="StaticPropertyOptions{TProvider}"/>.</item>
    /// <item><see cref="StatePropertyOptions{TProvider}"/>.</item>
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

        builder.Services
            .AddMapper<TProvider, EntryMapper<TProvider>>()
            .AddMapper<TProvider, EventIdMapper<TProvider>>()
            .AddMapper<TProvider, ExceptionMapper<TProvider>>()
            .AddMapper<TProvider, EnvironmentMapper<TProvider>>()
            .AddMapper<TProvider, TimestampMapper<TProvider>>()
            .AddMapper<TProvider, StaticMapper<TProvider>>()
            .AddMapper<TProvider, StateMapper<TProvider>>();

        return builder;
    }

    private static IServiceCollection AddMapper<TProvider, TMapper>(this IServiceCollection services)
        where TMapper : class, ILogPropertyMapper<TProvider>
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<ILogPropertyMapper<TProvider>, TMapper>());
        return services;
    }
}
