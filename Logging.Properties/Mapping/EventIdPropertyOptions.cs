namespace Microsoft.Extensions.Logging.Properties.Mapping;

using System.Collections.Generic;

/// <summary>
/// Property options for logged <see cref="EventId"/> values for a specific provider.
/// </summary>
/// <typeparam name="TProvider">The associated provider type.</typeparam>
public class EventIdPropertyOptions<TProvider> : EventIdPropertyOptions
{
}

/// <summary>
/// Property options for logged <see cref="EventId"/> values.
/// </summary>
public class EventIdPropertyOptions
{
    /// <summary>
    /// Mappable fields of an <see cref="EventId"/>. 
    /// </summary>
    public enum EventIdField
    {
        /// <summary>
        /// Field for <see cref="EventId.Id"/>.
        /// </summary>
        Id,

        /// <summary>
        /// Field for <see cref="EventId.Name"/>.
        /// </summary>
        Name,
    }

    /// <summary>
    /// Gets the mappings of field to property name.
    /// </summary>
    public IDictionary<EventIdField, string> Mappings { get; } = new Dictionary<EventIdField, string>();
}
