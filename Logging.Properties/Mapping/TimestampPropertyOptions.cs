namespace Microsoft.Extensions.Logging.Properties.Mapping;

/// <summary>
/// Property options for logged date and time for a specific provider.
/// </summary>
/// <typeparam name="TProvider">The associated provider type.</typeparam>
public class TimestampPropertyOptions<TProvider> : TimestampPropertyOptions
{
}

/// <summary>
/// Property options for logged date and time.
/// </summary>
public class TimestampPropertyOptions
{
    /// <summary>
    /// Gets or sets the mapped property name.
    /// </summary>
    public string? Mapping { get; set; }
}
