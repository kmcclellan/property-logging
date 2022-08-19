namespace Microsoft.Extensions.Logging.Properties;

using System.Buffers;

public interface ILogPropertySerializer
{
    bool IsEnabled(LogLevel level);

    void Begin(IBufferWriter<byte> writer, LogLevel level, EventId eventId);

    void WriteMessage(IBufferWriter<byte> writer, string message);

    void WriteException(IBufferWriter<byte> writer, Exception exception);

    void WriteProperty(IBufferWriter<byte> writer, string key, object? value);

    void Finish(IBufferWriter<byte> writer);
}
