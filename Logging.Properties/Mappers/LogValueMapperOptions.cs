namespace Microsoft.Extensions.Logging.Properties.Mappers;

using System;
using System.Collections.Generic;

class LogValueMapperOptions<TProvider> : LogValueMapperOptions
{
}

class LogValueMapperOptions
{
    public ICollection<Mapping> Mappings { get; } = new HashSet<Mapping>();

    public class Mapping
    {
        public Mapping(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public Mapping(string name, Func<object?> map)
        {
            this.Name = name;
            this.Map = map;
        }

        public string Name { get; }

        public object? Value { get; }

        public Func<object?>? Map { get; }

        public override bool Equals(object? obj) =>
            obj is Mapping mapping &&
                this.Name == mapping.Name &&
                this.Value == mapping.Value &&
                this.Map == mapping.Map;

        public override int GetHashCode() => HashCode.Combine(this.Name, this.Value, this.Map);
    }
}
