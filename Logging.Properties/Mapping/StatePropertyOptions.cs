namespace Microsoft.Extensions.Logging.Properties.Mapping;

/// <summary>
/// Property options for logged state for a specific provider.
/// </summary>
/// <typeparam name="TProvider">The associated provider type.</typeparam>
public class StatePropertyOptions<TProvider> : StatePropertyOptions
{
}

/// <summary>
/// Property options for logged state.
/// </summary>
public class StatePropertyOptions
{
    /// <summary>
    /// Gets the state options by category name.
    /// </summary>
    /// <remarks>
    ///  Keys may be a prefix or contain a wildcard ("*").
    /// </remarks>
    public IDictionary<string, StateCategoryPropertyOptions> Categories { get; } =
        new Dictionary<string, StateCategoryPropertyOptions>();
}
