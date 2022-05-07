namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

/// <summary>
/// A base provider for logging named properties.
/// </summary>
public abstract class PropertyLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
{
    readonly IEnumerable<ILogPropertyMapper> mappers;
    readonly ActionBlock<IEnumerable<KeyValuePair<string, object?>>>? processor;

    IExternalScopeProvider? scopes;

    /// <summary>
    /// Initializes the provider with the given property mappers.
    /// </summary>
    /// <param name="mappers">The log property mappers.</param>
    /// <param name="blockOptions">Options for queued processing (or <see langword="null"/> to block).</param>
    protected PropertyLoggerProvider(
        IEnumerable<ILogPropertyMapper> mappers,
        ExecutionDataflowBlockOptions? blockOptions = null)
    {
        this.mappers = mappers;

        if (blockOptions != null)
        {
            this.processor = new(this.Log, blockOptions);
        }
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => this.scopes = scopeProvider;

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName) => new PropertyLogger(this, categoryName);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public virtual async ValueTask DisposeAsync()
    {
        await this.DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Writes properties for a log entry.
    /// </summary>
    /// <param name="properties">The log properties.</param>
    protected abstract void Log(IEnumerable<KeyValuePair<string, object?>> properties);

    /// <summary>
    /// Disposes and/or finalizes the instance.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to include managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && this.processor != null)
        {
            this.processor.Complete();
            this.processor.Completion.Wait();
        }
    }

    /// <summary>
    /// Disposes the instance asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous disposal.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (this.processor != null)
        {
            this.processor.Complete();
            await this.processor.Completion.ConfigureAwait(false);
        }
    }

    private void Log<TState>(LogEntry<TState> entry)
    {
        var properties = this.Map(entry);

        if (this.processor == null)
        {
            this.Log(properties);
        }
        else if (!this.processor.Post(properties))
        {
            if (this.processor.Completion.Exception != null)
            {
                throw this.processor.Completion.Exception;
            }

            throw new InvalidOperationException("Unable to queue properties for processing.");
        }
    }

    private IEnumerable<KeyValuePair<string, object?>> Map<TState>(LogEntry<TState> entry)
    {
        foreach (var mapper in this.mappers)
        {
            foreach (var property in mapper.Map(entry, this.scopes))
            {
                yield return property;
            }
        }
    }

    private class PropertyLogger : ILogger
    {
        readonly PropertyLoggerProvider provider;
        readonly string category;

        public PropertyLogger(PropertyLoggerProvider provider, string category)
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
            Func<TState, Exception?, string> formatter) =>
            this.provider.Log<TState>(new(logLevel, this.category, eventId, state, exception, formatter));
    }
}
