namespace Microsoft.Extensions.Logging.Properties;

public interface ILogMessageCollector : ILogCollector
{
    void AddMessage(string message);
}
