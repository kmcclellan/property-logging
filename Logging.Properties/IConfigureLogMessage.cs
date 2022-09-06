namespace Microsoft.Extensions.Logging.Properties;

interface IConfigureLogMessage<in TState>
{
    string? Category { get; }

    void Log(TState state, string message);
}
