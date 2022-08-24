namespace Microsoft.Extensions.Logging.Properties;

public interface ILogSerializer<T>
{
    IDisposable Begin(out T writer);
}

public interface ILogSerializer<T, out TProvider> : ILogSerializer<T>
{
}
