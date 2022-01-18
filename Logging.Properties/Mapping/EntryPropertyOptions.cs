namespace Microsoft.Extensions.Logging.Properties.Mapping;

using Microsoft.Extensions.Logging.Abstractions;

using System.Collections.Generic;

/// <summary>
/// Property options for the top-level <see cref="LogEntry{TState}"/> for a specific provider.
/// </summary>
/// <typeparam name="TProvider">The associated provider type.</typeparam>
public class EntryPropertyOptions<TProvider> : EntryPropertyOptions
{
}

/// <summary>
/// Property options for the top-level <see cref="LogEntry{TState}"/>.
/// </summary>
public class EntryPropertyOptions
{
    /// <summary>
    /// Mappable fields of a <see cref="LogEntry{TState}"/>. 
    /// </summary>
    public enum EntryField
    {
        /// <summary>
        /// Field for <see cref="LogEntry{TState}.LogLevel"/>.
        /// </summary>
        LogLevel,

        /// <summary>
        /// Field for <see cref="LogEntry{TState}.Category"/>.
        /// </summary>
        Category,

        /// <summary>
        /// Field for <see cref="LogEntry{TState}.EventId"/>.
        /// </summary>
        EventId,

        /// <summary>
        /// Field for <see cref="LogEntry{TState}.State"/>.
        /// </summary>
        State,

        /// <summary>
        /// Field for <see cref="LogEntry{TState}.Exception"/>.
        /// </summary>
        Exception,

        /// <summary>
        /// Field for the formatted log message.
        /// </summary>
        Message,
    }

    /// <summary>
    /// Gets the mappings of field to property name.
    /// </summary>
    public IDictionary<EntryField, string> Mappings { get; } = new Dictionary<EntryField, string>();
}
