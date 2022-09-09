namespace Microsoft.Extensions.Logging.Policies;

interface IConfigureLogProperty<in TState>
{
    string? Category { get; }

    void Log(TState state, string name, object? value);
}
