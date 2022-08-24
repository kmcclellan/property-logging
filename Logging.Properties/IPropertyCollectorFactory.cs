namespace Microsoft.Extensions.Logging.Properties;

public interface IPropertyCollectorFactory
{
    IPropertyCollector Create(string category);
}

public interface IPropertyCollectorFactory<out TProvider> : IPropertyCollectorFactory
{
}
