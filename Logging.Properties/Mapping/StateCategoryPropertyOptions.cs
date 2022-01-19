namespace Microsoft.Extensions.Logging.Properties.Mapping;

/// <summary>
/// Property options for logged state for specific log categories.
/// </summary>
public class StateCategoryPropertyOptions
{
    /// <summary>
    /// Gets the mappings from state key to property name.
    /// </summary>
    public IDictionary<string, string> Mappings { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets whether to include unmapped keys as properties.
    /// </summary>
    public bool IncludeOthers { get; set; }

    /// <summary>
    /// Gets or sets whether to map properties from logged scopes.
    /// </summary>
    public bool IncludeScopes { get; set; }
}

