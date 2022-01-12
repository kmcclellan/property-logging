namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class PropertyLoggerProvider<T> : ILoggerProvider, ISupportExternalScope where T : IPropertyLogger
{
    readonly ILogPropertyMapper<PropertyLoggerProvider<T>> mapper;
    readonly T propertyLogger;

    public PropertyLoggerProvider(ILogPropertyMapper<PropertyLoggerProvider<T>> mapper, T propertyLogger)
    {
        this.mapper = mapper;
        this.propertyLogger = propertyLogger;
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        if (this.mapper is ISupportExternalScope sm)
        {
            sm.SetScopeProvider(scopeProvider);
        }
    }

    public ILogger CreateLogger(string categoryName) => new LoggerAdapter(this, categoryName);

    public void Dispose()
    {
    }

    private void Log<TState>(LogEntry<TState> entry)
    {
        var properties = this.mapper.Map(entry);
        this.propertyLogger.Log(properties);
    }

    private class LoggerAdapter : ILogger
    {
        readonly PropertyLoggerProvider<T> provider;
        readonly string category;

        public LoggerAdapter(PropertyLoggerProvider<T> provider, string category)
        {
            this.provider = provider;
            this.category = category;
        }

        public IDisposable BeginScope<TState>(TState state) => throw new NotSupportedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var entry = new LogEntry<TState>(logLevel, this.category, eventId, state, exception, formatter);
            this.provider.Log(entry);
        }
    }
}
