namespace Microsoft.Extensions.Logging.Properties.Mapping;

using System.Collections.Generic;

/// <summary>
/// Property options for the logged <see cref="Environment"/> for a specific provider.
/// </summary>
/// <typeparam name="TProvider">The associated provider type.</typeparam>
public class EnvironmentPropertyOptions<TProvider> : EnvironmentPropertyOptions
{
}

/// <summary>
/// Property options for the logged <see cref="Environment"/>.
/// </summary>
public class EnvironmentPropertyOptions
{
    /// <summary>
    /// Mappable fields of <see cref="Environment"/>. 
    /// </summary>
    public enum EnvironmentField
    {
        /// <summary>
        /// Field for <see cref="Environment.MachineName"/>.
        /// </summary>
        MachineName,

        /// <summary>
        /// Field for <see cref="Environment.UserName"/>.
        /// </summary>
        UserName,

        /// <summary>
        /// Field for <see cref="Environment.Version"/>.
        /// </summary>
        Version,

        /// <summary>
        /// Field for <see cref="Environment.OSVersion"/>.
        /// </summary>
        OSVersion,

#if NET5_0_OR_GREATER
        /// <summary>
        /// Field for <see cref="Environment.ProcessId"/>.
        /// </summary>
#else
        /// <summary>
        /// Field for the current process ID.
        /// </summary>
#endif
        ProcessId,
    }

    /// <summary>
    /// Gets the mappings of field to property name.
    /// </summary>
    public IDictionary<EnvironmentField, string> Mappings { get; } = new Dictionary<EnvironmentField, string>();
}
