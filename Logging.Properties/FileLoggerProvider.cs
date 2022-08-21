namespace Microsoft.Extensions.Logging.Properties;

[ProviderAlias("File")]
public class FileLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string category)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
}
