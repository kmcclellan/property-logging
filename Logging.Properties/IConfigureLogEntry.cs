namespace Microsoft.Extensions.Logging.Properties;

interface IConfigureLogEntry<in TState>
{
    string? Category { get; }

    void Log(TState state, string category, LogLevel level, EventId id);
}
