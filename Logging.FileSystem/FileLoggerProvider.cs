namespace Microsoft.Extensions.Logging.FileSystem;

using Microsoft.Extensions.Logging.Properties;

/// <summary>
/// Provides loggers for writing to a file.
/// </summary>
[ProviderAlias("File")]
public sealed class FileLoggerProvider : PropertyLoggerProvider
{
    internal FileLoggerProvider(IPropertyCollectorFactory<FileLoggerProvider> collectors)
        : base(collectors)
    {
    }
}
