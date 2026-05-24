namespace Atlas.Lite.Caching.Services;

public static class CacheKey
{
    public static string Build(params string[] segments)
    {
        return string.Join(
            ':',
            segments
                .Where(segment => !string.IsNullOrWhiteSpace(segment))
                .Select(Normalize));
    }

    private static string Normalize(string segment)
    {
        return segment.Trim().Replace(' ', '-').ToLowerInvariant();
    }
}
