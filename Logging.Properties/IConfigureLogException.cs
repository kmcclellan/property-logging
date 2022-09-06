namespace Microsoft.Extensions.Logging.Properties;

interface IConfigureLogException<in TState>
{
    string? Category { get; }

    void Log(TState state, Exception exception);
}
