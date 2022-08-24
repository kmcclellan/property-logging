namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Options;

using System.Collections.Concurrent;

class PropertyCollectorFactory<TWriter, TProvider> : IPropertyCollectorFactory<TProvider>, IDisposable
{
    readonly IOptionsMonitor<PropertyCollectorOptions<TWriter, TProvider>> options;
    readonly IPropertySerializer<TWriter> serializer;
    readonly ConcurrentDictionary<string, LogWriteAction<TWriter>?> actions = new();
    readonly IDisposable reload;

    public PropertyCollectorFactory(
        IOptionsMonitor<PropertyCollectorOptions<TWriter, TProvider>> options,
        IPropertySerializer<TWriter, TProvider> serializer)
    {
        this.options = options;
        this.serializer = serializer;
        this.reload = options.OnChange(x => this.actions.Clear());
    }

    public IPropertyCollector Create(string category)
    {
        return new LogCollector(this, category);
    }

    public void Dispose()
    {
        this.reload.Dispose();
    }

    LogWriteAction<TWriter>? GetAction(string category)
    {
        if (!this.actions.TryGetValue(category, out var action))
        {
            action = new() { Category = category };

            foreach (var filter in this.options.CurrentValue.Actions)
            {
                if (filter.Category == null || LogCategory.Matches(category, filter.Category))
                {
                    action.OnEntry += filter.OnEntry;
                    action.OnMessage += filter.OnMessage;
                    action.OnException += filter.OnException;
                    action.OnProperty += filter.OnProperty;
                }
            }

            this.actions[category] = action;
        }

        return action;
    }

    class LogCollector : IPropertyCollector
    {
        readonly PropertyCollectorFactory<TWriter, TProvider> factory;
        readonly string category;

        public LogCollector(PropertyCollectorFactory<TWriter, TProvider> factory, string category)
        {
            this.factory = factory;
            this.category = category;
        }

        public bool IsEnabled(LogLevel level)
        {
            return this.factory.GetAction(this.category) != null;
        }

        public IPropertyEntry Begin(LogLevel level, EventId id)
        {
            var action = this.factory.GetAction(this.category);
            if (action != null)
            {
                var entry = this.factory.serializer.Begin(out var writer);
                action.OnEntry?.Invoke(writer, this.category, level, id);

                return new EntryCollector(action, entry, writer);
            }

            return this.Skip();
        }

        struct EntryCollector : IPropertyEntry
        {
            readonly LogWriteAction<TWriter> action;
            readonly IDisposable entry;
            readonly TWriter writer;

            public EntryCollector(LogWriteAction<TWriter> action, IDisposable entry, TWriter writer)
            {
                this.action = action;
                this.entry = entry;
                this.writer = writer;
            }

            public bool SkipMessage => this.action.OnMessage == null;

            public bool SkipProperties => this.action.OnProperty == null;

            public void AddException(Exception exception)
            {
                this.action.OnException?.Invoke(this.writer, exception);
            }

            public void AddMessage(string message)
            {
                this.action.OnMessage?.Invoke(this.writer, message);
            }

            public void AddProperty(string name, object? value)
            {
                this.action.OnProperty?.Invoke(this.writer, name, value);
            }

            public void Dispose()
            {
                this.entry.Dispose();
            }
        }
    }
}
