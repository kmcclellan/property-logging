namespace Microsoft.Extensions.Logging.Properties;

public interface ILogEntryCollector : ILogPropertyCollector
{
    public bool SkipMessage { get; }

    public bool SkipProperties { get; }

    public void AddMessage(string message);

    public void AddException(Exception exception);
}
