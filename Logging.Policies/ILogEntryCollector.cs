namespace Microsoft.Extensions.Logging.Policies;

/// <summary>
/// A collector of information from an individual log entry.
/// </summary>
public interface ILogEntryCollector : IDisposable
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
    /// Collects from a log message.
    /// </summary>
    /// <param name="message">The log message.</param>
    void AddMessage(string message);

    /// <summary>
    /// Collects from a logged exception.
    /// </summary>
    /// <param name="exception">The logged exception.</param>
    void AddException(Exception exception);

    /// <summary>
    /// Collects from a log property.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    void AddProperty(string name, object? value);
}
