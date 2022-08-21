namespace Microsoft.Extensions.Logging.Properties;

public class FileLoggingOptions
{
    public string Path { get; set; } = "{0}.{1}.log";

    public FileStreamOptions Stream { get; } = new()
    {
        Mode = FileMode.Append,
        Access = FileAccess.Write,
        Share = FileShare.Read,
    };
}
