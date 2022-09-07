namespace Microsoft.Extensions.Logging.Properties;

/// <summary>
/// Options for file logging.
/// </summary>
public class FileLoggingOptions
{
    /// <summary>
    /// Gets or sets the path to the output log file.
    /// </summary>
    /// <remarks>
    /// Supports application and environment as format items. Default is <c>{0}.{1}.log</c>.
    /// </remarks>
    public string Path { get; set; } = "{0}.{1}.log";

    /// <summary>
    /// Gets or sets the flush interval for buffered data.
    /// </summary>
    /// <remarks>
    /// Default is one second.
    /// </remarks>
    public TimeSpan FlushInterval { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the maximum number of bytes to buffer before flushing.
    /// </summary>
    /// <remarks>
    /// Default is 4096.
    /// </remarks>
    public int BufferSize { get; set; } = 4096;

    /// <summary>
    /// Gets or sets whether to include log scope data.
    /// </summary>
    public bool IncludeScopes { get; set; }
}
