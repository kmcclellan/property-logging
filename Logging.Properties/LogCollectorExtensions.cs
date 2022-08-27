namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// Extensions of <see cref="ILogCollector{TEntry}"/>.
/// </summary>
public static class LogCollectorExtensions
{
    /// <summary>
    /// Creates a logger from a log collector.
    /// </summary>
    /// <typeparam name="TEntry">The log collector entry type.</typeparam>
    /// <param name="collector">The target log collector.</param>
    /// <param name="scopes">An optional logging scope provider.</param>
    /// <returns>The collector logger.</returns>
    public static ILogger AsLogger<TEntry>(this ILogCollector<TEntry> collector, IExternalScopeProvider? scopes = null)
        where TEntry : ILogCollectorEntry
    {
        ArgumentNullException.ThrowIfNull(collector, nameof(collector));
        return new CollectorLogger<TEntry>(collector, scopes);
    }
}
