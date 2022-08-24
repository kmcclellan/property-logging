namespace Microsoft.Extensions.Logging.Properties;

public class LogCollectorOptions<TWriter, TProvider>
{
    public ICollection<LogWriteAction<TWriter>> Actions { get; } = new List<LogWriteAction<TWriter>>();
}
