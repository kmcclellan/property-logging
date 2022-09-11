namespace Microsoft.Extensions.Logging.Policies;

class LogProperties
{
    readonly AsyncLocal<PropertyState> state = new();

    public IDisposable Push(IReadOnlyList<KeyValuePair<string, object?>> properties)
    {
        return this.state.Value.Push(properties);
    }

    public Enumerator GetEnumerator()
    {
        return this.state.Value.GetEnumerator();
    }

    readonly struct Enumerator
    {
        readonly Stack<KeyValuePair<string, object?>>.Enumerator properties;

        public Enumerator(Stack<KeyValuePair<string, object?>> properties)
        {
            this.properties = properties.GetEnumerator();
        }

        public KeyValuePair<string, object?> Current => this.properties.Current;

        public bool MoveNext()
        {
            return this.properties.MoveNext();
        }

        public void Dispose()
        {
            this.properties.Dispose();
        }
    }

    class PropertyState : IDisposable
    {
        readonly Stack<int> counts = new();
        readonly Stack<KeyValuePair<string, object?>> values = new();
        readonly Popper popper;

        public PropertyState()
        {
            this.popper = new(this);
        }

        public IDisposable Push(IReadOnlyList<KeyValuePair<string, object?>> properties)
        {
            for (var i = 0; i < properties.Count; i++)
            {
                this.values.Push(properties[i]);
            }

            this.counts.Push(properties.Count);
            return this.popper;
        }

        public Enumerator GetEnumerator()
        {
            return new(this.state.Value.Properties);
        }

        class Popper : IDisposable
        {
            readonly PropertyState state;

            public Popper(PropertyState state)
            {
                this.state = state;
            }

            public void Dispose()
            {
                for (var n = this.state.counts.Pop(); n > 0; n--)
                {
                    this.state.values.Pop();
                }
            }
        }
    }
}
