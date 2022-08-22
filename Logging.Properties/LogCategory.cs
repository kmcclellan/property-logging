namespace Microsoft.Extensions.Logging.Properties;

public static class LogCategory
{
    public static bool Matches(string category, string pattern)
    {
        const StringComparison cmp = StringComparison.OrdinalIgnoreCase;
        var split = pattern.IndexOf('*', cmp);

        return split == -1
            ? category.StartsWith(pattern, cmp)
            : category.AsSpan().StartsWith(pattern.AsSpan(0, split), cmp) &&
                category.AsSpan().EndsWith(pattern.AsSpan(split + 1), cmp);
    }
}
