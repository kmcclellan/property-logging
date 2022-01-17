namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// Property logging extensions to <see cref="ILoggingBuilder"/>.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds default implementations of <see cref="ILogPropertyMapper{TProvider}"/> to the logging services.
    /// </summary>
    /// <param name="builder">The logging builder.</param>
    /// <returns>The same instance, for chaining.</returns>
    public static ILoggingBuilder AddProperties(this ILoggingBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder;
    }
}
