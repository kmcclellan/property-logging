namespace Microsoft.Extensions.Logging.Properties;

public interface ILogCollector : IDisposable
{
    void AddException(Exception exception);
}
