namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Maps log information to named properties for a specific provider.
/// </summary>
/// <typeparam name="TProvider">The associated logger provider type.</typeparam>
public interface ILogPropertyMapper<out TProvider> : ILogPropertyMapper
{
}

/// <summary>
/// Maps log information to named properties.
/// </summary>
public interface ILogPropertyMapper
{
    /// <summary>
    /// Maps named properties for a log entry.
    /// </summary>
    /// <typeparam name="TState">The logged state type.</typeparam>
    /// <param name="entry">The log entry.</param>
    /// <param name="scopes">A provider for log scopes to include when mapping.</param>
    /// <returns>The mapped properties.</returns>
    IEnumerable<KeyValuePair<string, object>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null);
}

