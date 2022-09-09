namespace Microsoft.Extensions.Logging.Policies;

using Microsoft.Extensions.ObjectPool;

/// <summary>
/// A logging policy using typed writers.
/// </summary>
public class LoggingPolicy<TWriter> : ILoggingPolicy<EntryPolicy<TWriter>>
    where TWriter : class
{
    readonly ObjectPool<TWriter> pool;
    readonly IEnumerable<IConfigureLogEntry<TWriter>> configureEntry;
    readonly IEnumerable<IConfigureLogMessage<TWriter>> configureMessage;
    readonly IEnumerable<IConfigureLogException<TWriter>> configureException;
    readonly IEnumerable<IConfigureLogProperty<TWriter>> configureProperty;

    internal LoggingPolicy(
        ObjectPool<TWriter> pool,
        IEnumerable<IConfigureLogEntry<TWriter>> configureEntry,
        IEnumerable<IConfigureLogMessage<TWriter>> configureMessage,
        IEnumerable<IConfigureLogException<TWriter>> configureException,
        IEnumerable<IConfigureLogProperty<TWriter>> configureProperty)
    {
        this.pool = pool;
        this.configureEntry = configureEntry;
        this.configureMessage = configureMessage;
        this.configureException = configureException;
        this.configureProperty = configureProperty;
    }

    /// <inheritdoc/>
    public ICategoryPolicy<EntryPolicy<TWriter>> ForCategory(string category)
    {
        var collectEntry = default(Action<TWriter, LogLevel, EventId>);
        var collectMessage = default(Action<TWriter, string>);
        var collectException = default(Action<TWriter, Exception>);
        var collectProperty = default(Action<TWriter, string, object?>);

        foreach (var configure in this.configureEntry)
        {
            if (LogCategory.Matches(category, configure.Category))
            {
                collectEntry += (x, y, z) => configure.Log(x, category, y, z);
            }
        }

        foreach (var configure in this.configureMessage)
        {
            if (LogCategory.Matches(category, configure.Category))
            {
                collectMessage += configure.Log;
            }
        }

        foreach (var configure in this.configureException)
        {
            if (LogCategory.Matches(category, configure.Category))
            {
                collectException += configure.Log;
            }
        }

        foreach (var configure in this.configureProperty)
        {
            if (LogCategory.Matches(category, configure.Category))
            {
                collectProperty += configure.Log;
            }
        }

        return new WriterPolicy(this.pool, collectEntry, collectMessage, collectException, collectProperty);
    }

    class WriterPolicy : ICategoryPolicy<EntryPolicy<TWriter>>
    {
        readonly ObjectPool<TWriter> pool;
        readonly Action<TWriter, LogLevel, EventId>? entryAction;
        readonly Action<TWriter, string>? messageAction;
        readonly Action<TWriter, Exception>? exceptionAction;
        readonly Action<TWriter, string, object?>? propertyAction;
        readonly Action<TWriter>? finishAction;

        public WriterPolicy(
            ObjectPool<TWriter> pool,
            Action<TWriter, LogLevel, EventId>? entryAction,
            Action<TWriter, string>? messageAction,
            Action<TWriter, Exception>? exceptionAction,
            Action<TWriter, string, object?>? propertyAction)
        {
            this.pool = pool;
            this.entryAction = entryAction;
            this.messageAction = messageAction;
            this.exceptionAction = exceptionAction;
            this.propertyAction = propertyAction;
            this.finishAction = pool.Return;
        }

        public bool IsEnabled(LogLevel level)
        {
            return this.entryAction != null ||
                this.messageAction != null ||
                this.exceptionAction != null ||
                this.propertyAction != null;
        }

        public EntryPolicy<TWriter> Begin(LogLevel level, EventId id)
        {
            if (this.IsEnabled(level))
            {
                var writer = this.pool.Get();
                this.entryAction?.Invoke(writer, level, id);
                return new(writer, this.messageAction, this.exceptionAction, this.propertyAction, this.finishAction);
            }

            return default;
        }
    }
}
