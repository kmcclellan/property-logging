namespace Microsoft.Extensions.Logging.Policies;

/// <summary>
/// A base implementation of a logger provider using <see cref="ILogCollector{TEntry}"/>.
/// </summary>
/// <typeparam name="TEntry">The entry collector type.</typeparam>
public abstract class CollectorLoggerProvider<TEntry> : ILoggerProvider
    where TEntry : ILogEntryCollector
{
    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        return new CollectorLogger<TEntry>(this.GetCollector(categoryName));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets a collector to use for logging.
    /// </summary>
    /// <param name="category">The log category name.</param>
    /// <returns>The log collector.</returns>
    protected abstract ILogCollector<TEntry> GetCollector(string category);

    /// <summary>
    /// Disposes/releases resources used by the service.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to dispose managed resources,
    /// <see langword="false"/> to release unmanaged resources only.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
    }
}
