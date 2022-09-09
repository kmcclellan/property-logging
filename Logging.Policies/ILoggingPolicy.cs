namespace Microsoft.Extensions.Logging.Policies;

/// <summary>
/// A policy for writing log information.
/// </summary>
/// <typeparam name="TEntry">The type of log entry policy.</typeparam>
public interface ILoggingPolicy<TEntry>
    where TEntry : IEntryPolicy
{
    /// <summary>
    /// Configures a policy for a specific log category.
    /// </summary>
    /// <param name="category">The log category name.</param>
    /// <returns>The category policy.</returns>
    ICategoryPolicy<TEntry> ForCategory(string category);
}
