namespace Microsoft.Extensions.Logging.Properties.Mappers;

using System.Collections.Generic;

class LogExceptionMapperOptions<TProvider> : LogExceptionMapperOptions
{
}

class LogExceptionMapperOptions
{
    public ICollection<Mapping> Mappings { get; } = new HashSet<Mapping>();

    public class Mapping
    {
        public Mapping(string name, Func<Exception, object?> map, bool isRecursive)
        {
            this.Name = name;
            this.Map = map;
            this.IsRecursive = isRecursive;
        }

        public string Name { get; }

        public Func<Exception, object?> Map { get; }

        public bool IsRecursive { get; }

        public override bool Equals(object? obj) =>
            obj is Mapping mapping &&
                this.Name == mapping.Name &&
                this.Map == mapping.Map &&
                this.IsRecursive == mapping.IsRecursive;

        public override int GetHashCode() => HashCode.Combine(this.Name, this.Map, this.IsRecursive);
    }
}
