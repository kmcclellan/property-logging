namespace Microsoft.Extensions.Logging.Properties;

public interface IPropertyEntry : IDisposable
{
    bool SkipMessage { get; }

    bool SkipProperties { get; }

    void AddMessage(string message);

    void AddException(Exception exception);

    void AddProperty(string name, object? value);
}
