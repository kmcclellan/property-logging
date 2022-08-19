namespace Microsoft.Extensions.Logging.Properties;

public interface ILogPropertyCollector : IDisposable
{
    void AddProperty(string name, object? value);
}
