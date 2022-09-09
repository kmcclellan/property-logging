namespace Microsoft.Extensions.Logging.Policies;

interface IConfigureLogException<in TState>
{
    string? Category { get; }

    void Log(TState state, Exception exception);
}
