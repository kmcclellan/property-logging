namespace Microsoft.Extensions.Logging.Properties;

public class PropertyLoggingOptions
{
    public bool IncludeScopes { get; set; }
}

public class PropertyLoggingOptions<TProvider> : PropertyLoggingOptions
{
}
