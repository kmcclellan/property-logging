namespace Microsoft.Extensions.Logging.Properties.Mapping;

/// <summary>
/// Property options for static values for a specific provider.
/// </summary>
/// <typeparam name="TProvider">The associated provider type.</typeparam>
public class StaticPropertyOptions<TProvider> : StaticPropertyOptions
{
}

/// <summary>
/// Property options for static values.
/// </summary>
public class StaticPropertyOptions
{
    /// <summary>
    /// Gets the property values.
    /// </summary>
    public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();
}
