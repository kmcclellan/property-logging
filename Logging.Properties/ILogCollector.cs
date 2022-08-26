namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// An incremental collector of log entry data.
/// </summary>
/// <typeparam name="TState">The type of the state associated with each log entry.</typeparam>
public interface ILogCollector<TState>
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
    /// Begins collecting from a log entry.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="id">The log event ID.</param>
    /// <returns>State associated with the entry, used to collect additional data.</returns>
    TState? Begin(LogLevel level, EventId id);

    /// <summary>
    /// Collects from a logged exception.
    /// </summary>
    /// <param name="state">The state associated with the entry.</param>
    /// <param name="exception">The logged exception.</param>
    void AddException(TState state, Exception exception);

    /// <summary>
    /// Finishes collecting from a log entry.
    /// </summary>
    /// <param name="state">The state associated with the entry.</param>
    void Finish(TState state);
}
