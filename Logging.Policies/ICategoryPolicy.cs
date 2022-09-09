namespace Microsoft.Extensions.Logging.Policies;

/// <summary>
/// A policy for writing log information of a specific category.
/// </summary>
public interface ICategoryPolicy : ICategoryPolicy<IEntryPolicy>
{
}

/// <summary>
/// A policy for writing log information of a specific category.
/// </summary>
/// <typeparam name="TEntry">The type of log entry policy.</typeparam>
public interface ICategoryPolicy<TEntry>
    where TEntry : IEntryPolicy
{
    /// <summary>
    /// Checks whether logging is enabled for the given level.
    /// </summary>
    /// <remarks>
    /// Avoids side-effects of calling <see cref="Begin"/>. Not necessarily invoked before writing.
    /// </remarks>
    /// <param name="level">The log level.</param>
    /// <returns><see langword="true"/> if enabled, otherwise <see langword="false"/>.</returns>
    bool IsEnabled(LogLevel level);

    /// <summary>
    /// Begins writing a log entry using this policy.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="id">The log event ID.</param>
    /// <returns>A policy for writing the remainder of the entry.</returns>
    TEntry Begin(LogLevel level, EventId id);
}
