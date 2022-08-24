namespace Microsoft.Extensions.Logging.Properties;

public class PropertyCollectorOptions<TWriter, TProvider>
{
    public ICollection<LogWriteAction<TWriter>> Actions { get; } = new List<LogWriteAction<TWriter>>();
}
