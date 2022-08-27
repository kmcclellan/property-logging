namespace Microsoft.Extensions.Logging.Properties;

using System.Collections.Generic;

/// <summary>
/// A value-type entry for log collectors using actions.
/// </summary>
/// <typeparam name="TState">The type of state associated with the entry.</typeparam>
public readonly struct LogCollectorEntry<TState> : ILogCollectorEntry, IEquatable<LogCollectorEntry<TState>>
    where TState : IDisposable
{
    readonly Action<TState, string>? addMessage;
    readonly Action<TState, Exception>? addException;
    readonly Action<TState, string, object?>? addProperty;
    readonly TState state;

    /// <summary>
    /// Initializes the entry.
    /// </summary>
    /// <param name="addMessage">The action to collect the log message, if any.</param>
    /// <param name="addException">The action to collect a logged exception, if any.</param>
    /// <param name="addProperty">The action to collect log properties, if any.</param>
    /// <param name="state">The log entry state.</param>
    public LogCollectorEntry(
        Action<TState, string>? addMessage,
        Action<TState, Exception>? addException,
        Action<TState, string, object?>? addProperty,
        TState state)
    {
        this.addMessage = addMessage;
        this.addException = addException;
        this.addProperty = addProperty;
        this.state = state;
    }

    /// <inheritdoc/>
    public bool SkipMessage => this.addMessage == null;

    /// <inheritdoc/>
    public bool SkipProperties => this.addProperty == null;

    /// <inheritdoc/>
    public void AddException(Exception exception)
    {
        this.addException?.Invoke(this.state, exception);
    }

    /// <inheritdoc/>
    public void AddMessage(string message)
    {
        this.addMessage?.Invoke(this.state, message);
    }

    /// <inheritdoc/>
    public void AddProperty(string name, object? value)
    {
        this.addProperty?.Invoke(this.state, name, value);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.state.Dispose();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is LogCollectorEntry<TState> entry && this.Equals(entry);
    }

    /// <inheritdoc/>
    public bool Equals(LogCollectorEntry<TState> other)
    {
        return this.addMessage == other.addMessage &&
            this.addException == other.addException &&
            this.addProperty == other.addProperty &&
            EqualityComparer<TState>.Default.Equals(this.state, other.state);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.addMessage, this.addException, this.addProperty, this.state);
    }

    /// <inheritdoc/>
    public static bool operator ==(LogCollectorEntry<TState> x, LogCollectorEntry<TState> y)
    {
        return x.Equals(y);
    }

    /// <inheritdoc/>
    public static bool operator !=(LogCollectorEntry<TState> x, LogCollectorEntry<TState> y)
    {
        return !x.Equals(y);
    }
}
