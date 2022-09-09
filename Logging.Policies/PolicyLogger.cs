namespace Microsoft.Extensions.Logging.Policies;

using System;

/// <summary>
/// A logger using <see cref="ILoggingPolicy{TEntry}"/>.
/// </summary>
/// <typeparam name="TEntry">The entry policy type.</typeparam>
public class PolicyLogger<TEntry> : ILogger
    where TEntry : ILogEntryPolicy
{
    readonly ILoggingPolicy<TEntry> policy;
    IExternalScopeProvider? scopes;

    /// <summary>
    /// Initializes the logger.
    /// </summary>
    /// <param name="policy">The logging policy.</param>
    public PolicyLogger(ILoggingPolicy<TEntry> policy)
    {
        this.policy = policy;
    }

    /// <summary>
    /// Sets the provider for log scopes.
    /// </summary>
    /// <param name="scopes">The scope provider, or <see langword="null"/> to clear.</param>
    public void SetScopes(IExternalScopeProvider? scopes)
    {
        this.scopes = scopes;
    }

    /// <inheritdoc/>
    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotSupportedException($"Use '{nameof(SetScopes)}(...)' instead.");
    }

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
    {
        return this.policy.IsEnabled(logLevel);
    }

    /// <inheritdoc/>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter, nameof(formatter));

        using var entry = this.policy.Begin(logLevel, eventId);

        if (!entry.SkipMessage)
        {
            entry.AddMessage(formatter(state, exception));
        }

        if (exception != null)
        {
            entry.AddException(exception);
        }

        if (!entry.SkipProperties)
        {
            this.scopes?.ForEachScope((x, y) => Collect(x, y), entry);
            Collect(state, entry);
        }
    }

    static void Collect<T>(T state, ILogEntryPolicy entry)
    {
        // Using generic type may avoid boxing.
        if (state is IEnumerable<KeyValuePair<string, object?>> properties)
        {
            foreach (var (key, value) in properties)
            {
                entry.AddProperty(key, value);
            }
        }
    }
}
