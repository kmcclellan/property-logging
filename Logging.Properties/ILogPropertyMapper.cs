namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Maps log information to named properties.
/// </summary>
/// <typeparam name="TProvider">The associated logger provider type.</typeparam>
public interface ILogPropertyMapper<out TProvider>
{
    /// <summary>
    /// Maps named properties for a log entry.
    /// </summary>
    /// <typeparam name="TState">The logged state type.</typeparam>
    /// <param name="entry">The log entry.</param>
    /// <returns>The mapped properties.</returns>
    IEnumerable<KeyValuePair<string, object>> Map<TState>(LogEntry<TState> entry);
}

