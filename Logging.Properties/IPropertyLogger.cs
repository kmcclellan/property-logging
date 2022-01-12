namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// Logs information using named properties.
/// </summary>
public interface IPropertyLogger
{
    /// <summary>
    /// Writes properties for a log entry.
    /// </summary>
    /// <param name="properties">The log properties.</param>
    void Log(IEnumerable<KeyValuePair<string, object>> properties);
}
