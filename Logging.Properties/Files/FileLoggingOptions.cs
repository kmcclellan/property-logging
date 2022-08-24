namespace Microsoft.Extensions.Logging.Properties.Files;

using System.Text;

public class FileLoggingOptions
{
    public string Path { get; set; } = "{0}.{1}.log";

    public byte[]? Delimiter { get; set; } = Encoding.ASCII.GetBytes(Environment.NewLine);

    public int BufferSize { get; set; } = 4096;
}
