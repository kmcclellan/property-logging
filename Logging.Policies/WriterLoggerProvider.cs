namespace Microsoft.Extensions.Logging.Policies;

/// <summary>
/// A base provider for loggers using typed writers.
/// </summary>
/// <typeparam name="TWriter">The log writer type.</typeparam>
public abstract class WriterLoggerProvider<TWriter> : ILoggerProvider, ISupportExternalScope
{
    readonly IEnumerable<IConfigureLogEntry<TWriter>> configureEntry;
    readonly IEnumerable<IConfigureLogMessage<TWriter>> configureMessage;
    readonly IEnumerable<IConfigureLogException<TWriter>> configureException;
    readonly IEnumerable<IConfigureLogProperty<TWriter>> configureProperty;

    /// <summary>
    /// Initializes the provider.
    /// </summary>
    /// <param name="configureEntry">Configuration for writing log entries.</param>
    /// <param name="configureMessage">Configuration for writing log messages.</param>
    /// <param name="configureException">Configuration for writing logged exception.</param>
    /// <param name="configureProperty">Configuration for writing log properties.</param>
    protected WriterLoggerProvider(
        IEnumerable<IConfigureLogEntry<TWriter>> configureEntry,
        IEnumerable<IConfigureLogMessage<TWriter>> configureMessage,
        IEnumerable<IConfigureLogException<TWriter>> configureException,
        IEnumerable<IConfigureLogProperty<TWriter>> configureProperty)
    {
        this.configureEntry = configureEntry;
        this.configureMessage = configureMessage;
        this.configureException = configureException;
        this.configureProperty = configureProperty;
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets a writer to use for logging.
    /// </summary>
    /// <returns>The log writer.</returns>
    protected abstract TWriter GetWriter();

    /// <summary>
    /// Finishes writing a log entry.
    /// </summary>
    protected virtual void Finish(TWriter writer)
    {
    }

    /// <summary>
    /// Disposes and/or finalizes the instance.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to dispose and finalize, <see langword="false"/> to finalize only.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
    }
}

public interface IConfigureLogEntry<in TWriter>
{
    string? Category { get; }

    void Log(TWriter writer, string category, LogLevel level, EventId id);
}

public interface IConfigureLogMessage<in TWriter>
{
    string? Category { get; }

    void Log(TWriter writer, string message);
}

public interface IConfigureLogException<in TWriter>
{
    string? Category { get; }

    void Log(TWriter writer, Exception exception);
}

public interface IConfigureLogProperty<in TWriter>
{
    string? Category { get; }

    void Log(TWriter writer, string name, object? value);
}
