namespace Microsoft.Extensions.Logging.Properties;

interface IConfigureLogEntry<in TState>
{
    string? Category { get; }

    void Log(TState state, string category, LogLevel level, EventId id);
}

interface IConfigureLogMessage<in TState>
{
    string? Category { get; }

    void Log(TState state, string message);
}

interface IConfigureLogException<in TState>
{
    string? Category { get; }

    void Log(TState state, Exception exception);
}

interface IConfigureLogProperty<in TState>
{
    string? Category { get; }

    void Log(TState state, string name, object? value);
}
