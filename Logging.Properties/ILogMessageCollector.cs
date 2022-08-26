namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// An incremental collector of log messages.
/// </summary>
/// <typeparam name="TState">The type of the state associated with each log entry.</typeparam>
public interface ILogMessageCollector<TState> : ILogCollector<TState>
{
    /// <summary>
    /// Collects from a log message.
    /// </summary>
    /// <param name="state">The state associated with the entry.</param>
    /// <param name="message">The log message.</param>
    void AddMessage(TState state, string message);
}
