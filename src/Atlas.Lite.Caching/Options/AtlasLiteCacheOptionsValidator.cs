using Microsoft.Extensions.Options;

namespace Atlas.Lite.Caching.Options;

public sealed class AtlasLiteCacheOptionsValidator : IValidateOptions<AtlasLiteCacheOptions>
{
    public ValidateOptionsResult Validate(string? name, AtlasLiteCacheOptions options)
    {
        var failures = new List<string>();

        if (!IsProvider(options.Provider, CacheProviders.Memory) &&
            !IsProvider(options.Provider, CacheProviders.Redis))
        {
            failures.Add("AtlasLite:Cache:Provider must be Memory or Redis.");
        }

        if (options.DefaultExpirationSeconds <= 0)
        {
            failures.Add("AtlasLite:Cache:DefaultExpirationSeconds must be greater than zero.");
        }

        if (IsProvider(options.Provider, CacheProviders.Redis) &&
            string.IsNullOrWhiteSpace(options.Redis.ConnectionString))
        {
            failures.Add("AtlasLite:Cache:Redis:ConnectionString is required when Redis cache is enabled.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static bool IsProvider(string provider, string expected)
    {
        return string.Equals(provider, expected, StringComparison.OrdinalIgnoreCase);
    }
}
