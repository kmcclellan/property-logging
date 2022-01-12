namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Property logging extensions to <see cref="ILoggingBuilder"/>.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds <see cref="ILogPropertyMapper{TProvider}"/> to the logging services.
    /// </summary>
    /// <typeparam name="TProvider">The logger provider type.</typeparam>
    /// <param name="builder">The logging builder.</param>
    /// <returns>A builder to configure log properties.</returns>
    public static ILogPropertyBuilder AddProperties<TProvider>(this ILoggingBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<ILogPropertyMapper<TProvider>, LogPropertyMapper<TProvider>>());

        return new LogPropertyBuilder<TProvider>(builder.Services);
    }

    /// <summary>
    /// Adds a property logger to the factory.
    /// </summary>
    /// <typeparam name="T">The property logger type.</typeparam>
    /// <param name="builder">The logging builder.</param>
    /// <returns>A builder to configure log properties.</returns>
    public static ILogPropertyBuilder AddPropertyLogger<T>(this ILoggingBuilder builder)
        where T : class, IPropertyLogger
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services
            .AddSingleton<T>()
            .AddSingleton<ILoggerProvider, PropertyLoggerProvider<T>>();

        return builder.AddProperties<PropertyLoggerProvider<T>>();
    }
}
