namespace Microsoft.Extensions.Logging.Properties.Mapping;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Dependency injection.")]
class StateMapper<TProvider> : ILogPropertyMapper<TProvider>, IDisposable
{
    readonly ConcurrentDictionary<string, StateCategoryPropertyOptions> cache = new();
    readonly IDisposable reload;

    IEnumerable<KeyValuePair<string, StateCategoryPropertyOptions>> categories;

    public StateMapper(IOptionsMonitor<StatePropertyOptions<TProvider>> options)
    {
        this.categories = options.CurrentValue.Categories;
        this.reload = options.OnChange(
            opts =>
            {
                this.categories = opts.Categories;
                this.cache.Clear();
            });
    }

    public IEnumerable<KeyValuePair<string, object?>> Map<TState>(
        LogEntry<TState> entry,
        IExternalScopeProvider? scopes = null)
    {
        if (!this.cache.TryGetValue(entry.Category, out var options))
        {
            options = this.GetOptions(entry.Category);
            this.cache[entry.Category] = options;
        }

        var properties = default(ICollection<KeyValuePair<string, object?>>);

        if (options.Mappings.Count > 0 || options.IncludeOthers)
        {
            if (scopes != null && options.IncludeScopes)
            {
                scopes.ForEachScope(
                    (o, s) => MapState(o, s.options, ref s.properties),
                    (options, properties));
            }

            MapState(entry.State, options, ref properties);
        }

        return properties ?? Enumerable.Empty<KeyValuePair<string, object?>>();
    }

    public void Dispose() => this.reload.Dispose();

    private StateCategoryPropertyOptions GetOptions(string category)
    {
        var options = default(StateCategoryPropertyOptions);
        var length = 0;

        foreach (var kvp in this.categories)
        {
            // Use category matching logic from filter rules:
            // https://github.com/dotnet/runtime/blob/8fe0467efd29d952fc2afbf302c1d8a1bb3b2fa5/src/libraries/Microsoft.Extensions.Logging/src/LoggerRuleSelector.cs
            const char WildcardChar = '*';

#if NETCOREAPP2_1_OR_GREATER
            int wildcardIndex = kvp.Key.IndexOf(WildcardChar, StringComparison.Ordinal);
#else
            int wildcardIndex = kvp.Key.IndexOf(WildcardChar);
#endif

            if (wildcardIndex != -1 &&
                kvp.Key.IndexOf(WildcardChar, wildcardIndex + 1) != -1)
            {
                throw new InvalidOperationException("Only one wildcard character is allowed in category name.");
            }

            ReadOnlySpan<char> prefix, suffix;
            if (wildcardIndex == -1)
            {
                prefix = kvp.Key.AsSpan();
                suffix = default;
            }
            else
            {
                prefix = kvp.Key.AsSpan(0, wildcardIndex);
                suffix = kvp.Key.AsSpan(wildcardIndex + 1);
            }

            if (category.AsSpan().StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                category.AsSpan().EndsWith(suffix, StringComparison.OrdinalIgnoreCase) &&
                kvp.Key.Length > length)
            {
                options = kvp.Value;
                length = kvp.Key.Length;
            }
        }

        return options ?? new();
    }

    private static void MapState(
        object? state,
        StateCategoryPropertyOptions options,
        ref ICollection<KeyValuePair<string, object?>>? properties)
    {
        if (state is IEnumerable<KeyValuePair<string, object?>> values)
        {
            foreach (var kvp in values)
            {
                if (options.Mappings.TryGetValue(kvp.Key, out var mapping) || options.IncludeOthers)
                {
                    properties ??= new List<KeyValuePair<string, object?>>();
                    properties.Add(mapping != null ? new(mapping, kvp.Value) : kvp);
                }
            }
        }
    }
}
