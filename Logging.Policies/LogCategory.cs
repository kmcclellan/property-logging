namespace Microsoft.Extensions.Logging.Policies;

static class LogCategory
{
    public static bool Matches(ReadOnlySpan<char> category, ReadOnlySpan<char> pattern)
    {
        const char wildcard = '*';
        const StringComparison cmp = StringComparison.OrdinalIgnoreCase;

        var split = pattern.IndexOf(wildcard);
        if (split < 0)
        {
            return category.StartsWith(pattern, cmp);
        }

        var suffix = pattern[(split + 1)..];
        if (suffix.IndexOf(wildcard) > 0)
        {
            throw new ArgumentException(
                "Only one wildcard character is allowed in category name.",
                nameof(pattern));
        }

        return category.StartsWith(pattern[..split], cmp) && category.EndsWith(suffix, cmp);
    }
}
