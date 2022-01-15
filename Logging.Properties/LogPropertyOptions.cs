namespace Microsoft.Extensions.Logging.Properties;

class LogPropertyOptions<TProvider>
{
    public ICollection<ILogPropertyMapper> Mappers { get; } = new HashSet<ILogPropertyMapper>();
}
