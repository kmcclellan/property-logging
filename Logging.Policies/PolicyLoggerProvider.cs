namespace Microsoft.Extensions.Logging.Policies;

/// <summary>
/// Creates loggers using <see cref="ILoggingPolicy{TEntry}"/>.
/// </summary>
/// <typeparam name="TEntry">The logging policy entry type.</typeparam>
public abstract class PolicyLoggerProvider<TEntry> : ILoggerProvider, ISupportExternalScope
    where TEntry : ILogEntryPolicy
{
    readonly bool includeScopes;
    IExternalScopeProvider? scopes;

    /// <summary>
    /// Initializes the provider.
    /// </summary>
    /// <param name="includeScopes">Whether to include scopes when writing logs.</param>
    protected PolicyLoggerProvider(bool includeScopes = false)
    {
        this.includeScopes = includeScopes;
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        if (this.includeScopes)
        {
            this.scopes = scopeProvider;
        }
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        return new PolicyLogger(this, this.GetPolicy(categoryName));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Retrieves a policy for the given log category.
    /// </summary>
    /// <param name="category">The log category name.</param>
    /// <returns>The logging policy.</returns>
    protected abstract ILoggingPolicy<TEntry> GetPolicy(string category);

    /// <summary>
    /// Disposes and/or finalizes the instance.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to dispose and finalize, <see langword="false"/> to finalize only.</param>
    protected virtual void Dispose(bool disposing)
    {
    }

    class PolicyLogger : ILogger
    {
        readonly PolicyLoggerProvider<TEntry> provider;
        readonly ILoggingPolicy<TEntry> policy;

        public PolicyLogger(PolicyLoggerProvider<TEntry> provider, ILoggingPolicy<TEntry> policy)
        {
            this.provider = provider;
            this.policy = policy;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return this.policy.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException($"Use '{nameof(IExternalScopeProvider)}(...)' instead.");
        }

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
                this.provider.scopes?.ForEachScope((x, y) => Collect(x, y), entry);
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
}
