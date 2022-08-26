namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// An incremental collector of log properties.
/// </summary>
/// <typeparam name="TState">The type of the state associated with each log entry.</typeparam>
public interface ILogPropertyCollector<TState> : ILogCollector<TState>
{
    /// <summary>
    /// Collects from a log property.
    /// </summary>
    /// <param name="state">The state associated with the entry.</param>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    void AddProperty(TState state, string name, object? value);
}
