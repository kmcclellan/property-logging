namespace Microsoft.Extensions.Logging.Properties.Mappers;

using System;
using System.Collections.Generic;

class LogEntryMapperOptions<TProvider> : LogEntryMapperOptions
{
}

class LogEntryMapperOptions
{
    public enum MappingType
    {
        Level = 1,
        Category,
        EventId,
        State,
        Exception,
        Message
    }

    public ICollection<Mapping> Mappings { get; } = new HashSet<Mapping>();

    public class Mapping
    {
        public Mapping(string name, MappingType type)
        {
            this.Name = name;
            this.Type = type;
        }

        public string Name { get; }

        public MappingType Type { get; }

        public override bool Equals(object? obj) =>
            obj is Mapping mapping && this.Type == mapping.Type && this.Name == mapping.Name;

        public override int GetHashCode() => HashCode.Combine(this.Type, this.Name);
    }
}
