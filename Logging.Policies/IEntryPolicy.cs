namespace Microsoft.Extensions.Logging.Policies;

/// <summary>
/// A policy for writing information for a single log entry.
/// </summary>
public interface IEntryPolicy : IDisposable
{
    /// <summary>
    /// Gets whether to skip the message for this entry.
    /// </summary>
    bool SkipMessage { get; }

    /// <summary>
    /// Gets whether to skip properties for this entry.
    /// </summary>
    bool SkipProperties{ get; }

    /// <summary>
    /// Adds a message to the log entry.
    /// </summary>
    /// <param name="message">The log message.</param>
    void AddMessage(string message);

    /// <summary>
    /// Adds an exception to the log entry.
    /// </summary>
    /// <param name="exception">The logged exception.</param>
    void AddException(Exception exception);

    /// <summary>
    /// Adds a property to the log entry.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    void AddProperty(string name, object? value);
}
