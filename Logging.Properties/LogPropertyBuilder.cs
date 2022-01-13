namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Collections.Generic;

class LogPropertyBuilder<TProvider> : ILogPropertyBuilder
{
    readonly IServiceCollection services;

    public LogPropertyBuilder(IServiceCollection services)
    {
        this.services = services;
    }

    public ILogPropertyBuilder FromEntry(
        string? level = null,
        string? category = null,
        string? eventId = null,
        string? state = null,
        string? exception = null,
        string? message = null)
    {
        TryBuild(level, s => new LevelProperty(s));
        TryBuild(category, s => new CategoryProperty(s));
        TryBuild(eventId, s => new EventIdProperty(s));
        TryBuild(state, s => new StateProperty(s));
        TryBuild(exception, s => new ExceptionProperty(s));
        TryBuild(message, s => new MessageProperty(s));

        void TryBuild(string? name, Func<string, ILogProperty> build)
        {
            if (name != null)
            {
                this.Add(build(name));
            }
        }

        return this;
    }

    public ILogPropertyBuilder FromState(string name, Func<object, object?> map, bool scopes = false)
{
        this.Add(new StateProperty(name, map, scopes));
        return this;
    }

    public ILogPropertyBuilder FromState<T>(string name, Func<T, object?> map, bool scopes = false) =>
        this.FromState(name, x => x is T t ? map(t) : null, scopes);

    public ILogPropertyBuilder FromException(string name, Func<Exception, object?> map, bool inner = false)
    {
        this.Add(new ExceptionProperty(name, map, inner));
        return this;
    }

    public ILogPropertyBuilder FromException<T>(string name, Func<Exception, object?> map, bool inner = false)
        where T : Exception =>
        this.FromException(name, x => x is T t ? map(t) : null, inner);


    public ILogPropertyBuilder FromValue(string name, object value)
    {
        this.Add(new ValueProperty(name, value));
        return this;
    }

    public ILogPropertyBuilder FromValue(string name, Func<object?> map)
    {
        this.Add(new ValueProperty(name, map));
        return this;
    }

    private void Add(ILogProperty property)
    {
        this.services.AddSingleton(property);
    }

    private class LevelProperty : LogProperty
    {
        public LevelProperty(string name) : base(name)
        {
        }

        protected override object? GetValue<TState>(LogEntry<TState> entry, IExternalScopeProvider? scopes) =>
            entry.LogLevel;
    }


    private class CategoryProperty : LogProperty
    {
        public CategoryProperty(string name) : base(name)
        {
        }

        protected override object? GetValue<TState>(LogEntry<TState> entry, IExternalScopeProvider? scopes) =>
            entry.Category;
    }


    private class EventIdProperty : LogProperty
    {
        public EventIdProperty(string name) : base(name)
        {
        }

        protected override object? GetValue<TState>(LogEntry<TState> entry, IExternalScopeProvider? scopes) =>
            entry.EventId;
    }

    private class StateProperty : LogProperty
    {
        readonly Func<object, object?>? map;
        readonly bool scopes;

        public StateProperty(string name) : base(name)
        {
        }

        public StateProperty(string name, Func<object, object?>? map, bool scopes) : base(name)
        {
            this.map = map;
            this.scopes = scopes;
        }


        public override IEnumerable<object> GetValues<TState>(LogEntry<TState> entry, IExternalScopeProvider? scopes)
        {
            if (this.map != null && scopes != null && this.scopes)
            {
                var values = new List<object>();

                scopes.ForEachScope(
                    (obj, item) =>
                    {
                        if (obj != null)
                        {
                            var value = item.map(obj);

                            if (value != null)
                            {
                                item.values.Add(value);
                            }
                        }
                    },
                    (this.map, values));

                foreach (var scopeValue in values)
                {
                    yield return scopeValue;
                }
            }

            if (entry.State != null)
            {
                if (this.map == null)
                {
                    yield return entry.State;
                }
                else
                {
                    var value = this.map(entry.State);

                    if (value != null)
                    {
                        yield return value;
                    }
                }
            }
        }
    }

    private class ExceptionProperty : LogProperty
    {
        readonly Func<Exception, object?>? map;
        readonly bool inner;

        public ExceptionProperty(string name) : base(name)
        {
        }

        public ExceptionProperty(string name, Func<Exception, object?>? map, bool inner) : base(name)
        {
            this.map = map;
            this.inner = inner;
        }

        public override IEnumerable<object> GetValues<TState>(LogEntry<TState> entry, IExternalScopeProvider? scopes)
        {
            if (this.map != null && this.inner)
            {
                foreach (var exception in Flatten(entry.Exception))
                {
                    var value = this.map(exception);

                    if (value != null)
                    {
                        yield return value;
                    }
                }
            }
            else if (entry.Exception != null)
            {
                var value = this.map == null ? entry.Exception : this.map(entry.Exception);

                if (value != null)
                {
                    yield return value;
                }
            }
        }

        private static IEnumerable<Exception> Flatten(Exception? exception)
        {
            if (exception is AggregateException agg)
            {
                foreach (var inner1 in agg.InnerExceptions)
                {
                    foreach (var inner2 in Flatten(inner1))
                    {
                        yield return inner2;
                    }
                }
            }
            else if (exception != null)
            {
                yield return exception;

                foreach (var inner in Flatten(exception.InnerException))
                {
                    yield return inner;
                }
            }
        }
    }

    private class MessageProperty : LogProperty
    {
        public MessageProperty(string name) : base(name)
        {
        }

        protected override object? GetValue<TState>(LogEntry<TState> entry, IExternalScopeProvider? scopes) =>
            entry.Formatter?.Invoke(entry.State, entry.Exception);
    }

    private class ValueProperty : LogProperty
    {
        readonly object? value;
        readonly Func<object?>? map;

        public ValueProperty(string name, object value) : base(name)
        {
            this.value = value;
        }

        public ValueProperty(string name, Func<object?> map) : base(name)
        {
            this.map = map;
        }

        protected override object? GetValue<TState>(LogEntry<TState> entry, IExternalScopeProvider? scopes) =>
            this.value ?? this.map?.Invoke();
    }

    private abstract class LogProperty : ILogProperty
    {
        public LogProperty(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public Type ProviderType => typeof(TProvider);

        public virtual IEnumerable<object> GetValues<TState>(LogEntry<TState> entry, IExternalScopeProvider? scopes)
        {
            var value = GetValue(entry, scopes);

            if (value != null)
            {
                yield return value;
            }
        }

        protected virtual object? GetValue<TState>(LogEntry<TState> entry, IExternalScopeProvider? scopes) => null;
    }
}
