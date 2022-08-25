namespace Microsoft.Extensions.Logging.Properties;

public class PropertyCollectorOptions<TWriter>
{
    public ICollection<LogWriteAction<TWriter>> Actions { get; } = new List<LogWriteAction<TWriter>>();
}

public class PropertyCollectorOptions<TWriter, TProvider> : PropertyCollectorOptions<TWriter>
{
}
