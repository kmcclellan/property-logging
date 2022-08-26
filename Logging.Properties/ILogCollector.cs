namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// An incremental collector of log data.
/// </summary>
public interface ILogCollector
{
    /// <summary>
    /// Checks whether the collector is enabled for the given level.
    /// </summary>
    /// <remarks>
    /// Useful to avoid side-effects of calling <see cref="Begin"/>. Not necessary to invoke before collecting.
    /// </remarks>
    /// <param name="level">The log level.</param>
    /// <returns><see langword="true"/> if enabled, otherwise <see langword="false"/>.</returns>
    bool IsEnabled(LogLevel level);

    /// <summary>
    /// Begins collecting for a log entry.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="id">The log event ID.</param>
    /// <returns>A collector for the entry.</returns>
    ILogCollectorEntry Begin(LogLevel level, EventId id);
}
