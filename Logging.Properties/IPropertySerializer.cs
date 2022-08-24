namespace Microsoft.Extensions.Logging.Properties;

public interface IPropertySerializer<T>
{
    IDisposable Begin(out T writer);
}

public interface IPropertySerializer<T, out TProvider> : IPropertySerializer<T>
{
}
