namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// Extensions of <see cref="ILogCollector"/>.
/// </summary>
public static class LogCollectorExtensions
{
    /// <summary>
    /// Creates a logger from a log collector.
    /// </summary>
    /// <param name="collector">The target log collector.</param>
    /// <param name="scopes">An optional logging scope provider.</param>
    /// <returns>The collector logger.</returns>
    public static ILogger AsLogger(this ILogCollector collector, IExternalScopeProvider? scopes = null)
    {
        ArgumentNullException.ThrowIfNull(collector, nameof(collector));
        return new CollectorLogger(collector, scopes);
    }
}
