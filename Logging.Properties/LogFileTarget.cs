namespace Microsoft.Extensions.Logging.Properties;

using System.Buffers;
using System.Globalization;
using System.Text;
using System.Threading.Tasks.Dataflow;

class LogFileTarget<T> : ITargetBlock<T>
{
    static readonly byte[] Delimiter = Encoding.UTF8.GetBytes(Environment.NewLine);

    static readonly FileStreamOptions WriteOptions = new()
    {
        Mode = FileMode.Append,
        Access = FileAccess.Write,
        Share = FileShare.Read,
        Options = FileOptions.Asynchronous,
        BufferSize = 0,
    };

    readonly ITargetBlock<T> target;

    public LogFileTarget(
        string path,
        Action<IBufferWriter<byte>, T> writeAction,
        int bufferSize,
        TimeSpan flushInterval)
    {
        var buffer = new BufferBlock<T>();

        var flushTask = Task.Factory.StartNew(
            () => this.Flush(path, writeAction, bufferSize, flushInterval, buffer),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Default);

        this.Completion = Task.Factory.ContinueWhenAll(
            new[] { buffer.Completion, flushTask },
            t => t[0].IsFaulted ? t[0] : t[1],
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default)
            .Unwrap();

        this.target = buffer;
    }

    public Task Completion { get; }

    public void Complete()
    {
        this.target.Complete();
    }

    public void Fault(Exception exception)
    {
        this.target.Fault(exception);
    }

    public DataflowMessageStatus OfferMessage(
        DataflowMessageHeader messageHeader,
        T messageValue,
        ISourceBlock<T>? source,
        bool consumeToAccept)
    {
        return this.target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
    }

    async Task Flush(
        string path,
        Action<IBufferWriter<byte>, T> writeAction,
        int bufferSize,
        TimeSpan flushInterval,
        BufferBlock<T> buffer)
    {
        try
        {
            var writer = new ArrayBufferWriter<byte>();

            while (await buffer.OutputAvailableAsync().ConfigureAwait(false))
            {
                var timestamp = DateTime.UtcNow;

                do
                {
                    TimeSpan delay;

                    if (buffer.TryReceive(out var entry))
                    {
                        writeAction(writer, entry);
                        writer.Write(Delimiter);
                    }
                    else if (writer.WrittenCount < bufferSize &&
                        (delay = DateTime.UtcNow - timestamp + flushInterval) > TimeSpan.Zero)
                    {
                        try
                        {
                            await buffer.OutputAvailableAsync().WaitAsync(delay).ConfigureAwait(false);
                        }
                        catch (TimeoutException)
                        {
                        }
                    }
                    else
                    {
                        try
                        {
                            using var output = File.Open(path, WriteOptions);
                            await output.WriteAsync(writer.WrittenMemory).ConfigureAwait(false);
                        }
                        catch (IOException exception)
                        {
                            await Console.Error.WriteLineAsync(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Error flushing logs ({0} KiB). {1}",
                                    writer.WrittenCount >> 10,
                                    exception))
                                .ConfigureAwait(false);

                            await Task.Delay(100).ConfigureAwait(false);
                            continue;
                        }

                        writer.Clear();
                    }
                }
                while (writer.WrittenCount > 0);
            }
        }
        catch (Exception exception)
        {
            this.target.Fault(exception);
            throw;
        }
    }
}
