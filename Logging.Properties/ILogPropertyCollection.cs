namespace Microsoft.Extensions.Logging.Properties;

public interface ILogPropertyCollection : IDisposable
{
    bool SkipMessage { get; }

    bool SkipProperties { get; }

    void AddMessage(string message);

    void AddProperty(KeyValuePair<string, object?> kvp);
}
