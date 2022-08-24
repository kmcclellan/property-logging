namespace Microsoft.Extensions.Logging.Properties;

static class LogCategory
{
    public static bool Matches(string category, string pattern)
    {
        ArgumentNullException.ThrowIfNull(category, nameof(category));
        ArgumentNullException.ThrowIfNull(pattern, nameof(pattern));

        const StringComparison cmp = StringComparison.OrdinalIgnoreCase;
        var split = pattern.IndexOf('*', cmp);

        if (split > 0 && category.IndexOf('*', split + 1) > 0)
        {
            throw new ArgumentException(
                "Only one wildcard character is allowed in category name.",
                nameof(pattern));
        }

        return split == -1
            ? category.StartsWith(pattern, cmp)
            : category.AsSpan().StartsWith(pattern.AsSpan(0, split), cmp) &&
                category.AsSpan().EndsWith(pattern.AsSpan(split + 1), cmp);
    }
}
