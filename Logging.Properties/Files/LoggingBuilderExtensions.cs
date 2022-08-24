namespace Microsoft.Extensions.Logging.Properties.Files;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Configuration;

using System.Buffers;

public static class LoggingBuilderExtensions
{
    public static LogPropertyBuilder<FileLoggerProvider, IBufferWriter<byte>> AddFile(this ILoggingBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        LoggerProviderOptions.RegisterProviderOptions<FileLoggingOptions, FileLoggerProvider>(builder.Services);

        return new LogPropertyBuilder<FileLoggerProvider>(builder.Services)
            .Serialize<FileLogSerializer, IBufferWriter<byte>>();
    }
}
