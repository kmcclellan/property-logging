namespace Microsoft.Extensions.Logging.Properties;

public interface ILogPropertyCollector : ILogCollector
{
    void AddProperty(string name, object? value);
}
