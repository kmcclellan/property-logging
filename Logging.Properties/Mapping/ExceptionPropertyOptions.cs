namespace Microsoft.Extensions.Logging.Properties.Mapping;

using System.Collections.Generic;

/// <summary>
/// Property options for logged <see cref="Exception"/> values for a specific provider.
/// </summary>
/// <typeparam name="TProvider">The associated provider type.</typeparam>
public class ExceptionPropertyOptions<TProvider> : ExceptionPropertyOptions
{
}

/// <summary>
/// Property options for logged <see cref="Exception"/> values.
/// </summary>
public class ExceptionPropertyOptions
{
    /// <summary>
    /// Mappable fields of an <see cref="Exception"/>. 
    /// </summary>
    public enum ExceptionField
    {
        /// <summary>
        /// Field for the exception type.
        /// </summary>
        Type,

        /// <summary>
        /// Field for <see cref="Exception.Message"/>.
        /// </summary>
        Message,

        /// <summary>
        /// Field for <see cref="Exception.StackTrace"/>.
        /// </summary>
        StackTrace,

        /// <summary>
        /// Field for <see cref="Exception.Source"/>.
        /// </summary>
        Source,

        /// <summary>
        /// Field for <see cref="Exception.TargetSite"/>.
        /// </summary>
        TargetSite,

        /// <summary>
        /// Field for <see cref="Exception.HResult"/>.
        /// </summary>
        HResult,

        /// <summary>
        /// Field for <see cref="Exception.HelpLink"/>.
        /// </summary>
        HelpLink,
    }

    /// <summary>
    /// Gets the mappings of field to property name.
    /// </summary>
    public IDictionary<ExceptionField, string> Mappings { get; } = new Dictionary<ExceptionField, string>();

    /// <summary>
    /// Gets or sets whether to perform recursive mapping of inner exceptions.
    /// </summary>
    public bool IsRecursive { get; set; }
}
