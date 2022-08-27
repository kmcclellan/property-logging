namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// Extensions of <see cref="ILogCollector{TState}"/>.
/// </summary>
public static class LogCollectorExtensions
{
    /// <summary>
    /// Creates a logger from a log collector.
    /// </summary>
    /// <typeparam name="TState">The state type of the collector.</typeparam>
    /// <param name="collector">The target log collector.</param>
    /// <param name="scopes">An optional logging scope provider.</param>
    /// <returns>The collector logger.</returns>
    public static ILogger AsLogger<TState>(this ILogCollector<TState> collector, IExternalScopeProvider? scopes = null)
    {
        ArgumentNullException.ThrowIfNull(collector, nameof(collector));
        return new CollectorLogger<TState>(collector, scopes);
    }
}
