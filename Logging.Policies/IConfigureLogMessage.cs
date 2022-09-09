namespace Microsoft.Extensions.Logging.Policies;

interface IConfigureLogMessage<in TState>
{
    string? Category { get; }

    void Log(TState state, string message);
}
