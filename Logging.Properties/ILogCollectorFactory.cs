namespace Microsoft.Extensions.Logging.Properties;

public interface ILogCollectorFactory
{
    ILogCollector Create(string category);
}

public interface ILogCollectorFactory<out TProvider> : ILogCollectorFactory
{
}
