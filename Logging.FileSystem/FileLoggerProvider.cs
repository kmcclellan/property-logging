namespace Microsoft.Extensions.Logging.FileSystem;

using Microsoft.Extensions.Logging.Properties;
using Microsoft.Extensions.Options;

/// <summary>
/// Provides loggers for writing to a file.
/// </summary>
[ProviderAlias("File")]
public sealed class FileLoggerProvider : PropertyLoggerProvider
{
    internal FileLoggerProvider(
        IOptionsMonitor<PropertyLoggingOptions<FileLoggerProvider>> options,
        IPropertyCollectorFactory<FileLoggerProvider> collectors)
        : base(options, collectors)
    {
    }
}
