namespace Microsoft.Extensions.Logging.Properties;

using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Configures mappings from log information to named properties.
/// </summary>
public interface ILogPropertyBuilder
{
    /// <summary>
    /// Configures property mappings from the top-level <see cref="LogEntry{TState}"/>.
    /// </summary>
    /// <param name="level">
    /// The mapped property name for <see cref="LogEntry{TState}.LogLevel"/>, or <see langword="null"/>.
    /// </param>
    /// <param name="category">
    /// The mapped property name for <see cref="LogEntry{TState}.Category"/>, or <see langword="null"/>.
    /// </param>
    /// <param name="eventId">
    /// The mapped property name for <see cref="LogEntry{TState}.EventId"/>, or <see langword="null"/>.
    /// </param>
    /// <param name="state">
    /// The mapped property name for <see cref="LogEntry{TState}.State"/>, or <see langword="null"/>.
    /// </param>
    /// <param name="exception">
    /// The mapped property name for <see cref="LogEntry{TState}.Exception"/>, or <see langword="null"/>.
    /// </param>
    /// <param name="message">
    /// The mapped property name for the formatted log message, or <see langword="null"/>.
    /// </param>
    /// <returns>The same instance, for chaining.</returns>
    ILogPropertyBuilder FromEntry(
        string? level = null,
        string? category = null,
        string? eventId = null,
        string? state = null,
        string? exception = null,
        string? message = null);

    /// <summary>
    /// Configures a property mapping from logged exceptions.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="map">A delegate mapping the property value (<see langword="null"/> for no value).</param>
    /// <param name="inner"><see langword="true"/> to map inner exceptions (recursively).</param>
    /// <returns>The same instance, for chaining.</returns>
    ILogPropertyBuilder FromException(string name, Func<Exception, object?> map, bool inner = false);

    /// <summary>
    /// Configures a property mapping from logged exceptions of a given type.
    /// </summary>
    /// <typeparam name="T">The exception type.</typeparam>
    /// <param name="name">The property name.</param>
    /// <param name="map">A delegate mapping the property value (<see langword="null"/> for no value).</param>
    /// <param name="inner"><see langword="true"/> to map inner exceptions (recursively).</param>
    /// <returns>The same instance, for chaining.</returns>
    ILogPropertyBuilder FromException<T>(string name, Func<T, object?> map, bool inner = false)
        where T : Exception;

    /// <summary>
    /// Configures a property mapping from a static value.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    /// <returns>The same instance, for chaining.</returns>
    ILogPropertyBuilder FromValue(string name, object value);

    /// <summary>
    /// Configures a property mapping from a computed value.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="map">A delegate mapping the property value (<see langword="null"/> for no value).</param>
    /// <returns>The same instance, for chaining.</returns>
    ILogPropertyBuilder FromValue(string name, Func<object?> map);
}
