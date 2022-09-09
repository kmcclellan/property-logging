namespace Microsoft.Extensions.Logging.Policies;

using System.Collections.Generic;

/// <summary>
/// A log entry policy using a typed writer.
/// </summary>
/// <typeparam name="TWriter">The writer type.</typeparam>
public readonly struct EntryPolicy<TWriter> : IEntryPolicy, IEquatable<EntryPolicy<TWriter>>
{
    readonly Action<TWriter, string>? addMessage;
    readonly Action<TWriter, Exception>? addException;
    readonly Action<TWriter, string, object?>? addProperty;
    readonly Action<TWriter>? finish;
    readonly TWriter writer;

    /// <summary>
    /// Initializes the entry policy.
    /// </summary>
    /// <param name="writer">The log writer.</param>
    /// <param name="messageAction">The delegate to write a log message, if any.</param>
    /// <param name="exceptionAction">The delegate to write a logged exception, if any.</param>
    /// <param name="propertyAction">The delegate to write a log property, if any.</param>
    /// <param name="finishAction">The delegate to finish writing the log entry, if any.</param>
    public EntryPolicy(
        TWriter writer,
        Action<TWriter, string>? messageAction = null,
        Action<TWriter, Exception>? exceptionAction = null,
        Action<TWriter, string, object?>? propertyAction = null,
        Action<TWriter>? finishAction = null)
    {
        this.addMessage = messageAction;
        this.addException = exceptionAction;
        this.addProperty = propertyAction;
        this.finish = finishAction;
        this.writer = writer;
    }

    /// <inheritdoc/>
    public bool SkipMessage => this.addMessage == null;

    /// <inheritdoc/>
    public bool SkipProperties => this.addProperty == null;

    /// <inheritdoc/>
    public void AddMessage(string message)
    {
        this.addMessage?.Invoke(this.writer, message);
    }

    /// <inheritdoc/>
    public void AddException(Exception exception)
    {
        this.addException?.Invoke(this.writer, exception);
    }

    /// <inheritdoc/>
    public void AddProperty(string name, object? value)
    {
        this.addProperty?.Invoke(this.writer, name, value);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.finish?.Invoke(this.writer);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is EntryPolicy<TWriter> policy && this.Equals(policy);
    }

    /// <inheritdoc/>
    public bool Equals(EntryPolicy<TWriter> other)
    {
        return this.addMessage == other.addMessage &&
            this.addException == other.addException &&
            this.addProperty == other.addProperty &&
            this.finish == other.finish &&
            EqualityComparer<TWriter>.Default.Equals(this.writer, other.writer);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.addMessage, this.addException, this.addProperty, this.finish, this.writer);
    }

    /// <summary>
    /// Determines if entry policies have the same value.
    /// </summary>
    /// <param name="left">The left policy.</param>
    /// <param name="right">The right policy.</param>
    /// <returns><see langword="true"/> if value is the same, otherwise <see langword="false"/>.</returns>
    public static bool operator ==(EntryPolicy<TWriter> left, EntryPolicy<TWriter> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines if entry policies have different values.
    /// </summary>
    /// <param name="left">The left policy.</param>
    /// <param name="right">The right policy.</param>
    /// <returns><see langword="true"/> if values are different, otherwise <see langword="false"/>.</returns>
    public static bool operator !=(EntryPolicy<TWriter> left, EntryPolicy<TWriter> right)
    {
        return !(left == right);
    }
}
