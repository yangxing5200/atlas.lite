using System.ComponentModel.DataAnnotations;

namespace Atlas.Lite.Caching.Options;

public sealed class AtlasLiteCacheOptions
{
    public const string SectionName = "AtlasLite:Cache";

    [Required]
    public string Provider { get; set; } = CacheProviders.Memory;

    public int DefaultExpirationSeconds { get; set; } = 300;

    public RedisCacheOptions Redis { get; set; } = new();
}

public sealed class RedisCacheOptions
{
    public string? ConnectionString { get; set; }
}

public static class CacheProviders
{
    public const string Memory = "Memory";
    public const string Redis = "Redis";
}
