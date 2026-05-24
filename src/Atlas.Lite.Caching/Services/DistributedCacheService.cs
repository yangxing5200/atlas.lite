using System.Text.Json;
using Atlas.Lite.Caching.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Atlas.Lite.Caching.Services;

public sealed class DistributedCacheService(
    IDistributedCache distributedCache,
    IOptions<AtlasLiteCacheOptions> options) : ICacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var payload = await distributedCache.GetStringAsync(key, cancellationToken);
        return payload is null ? default : JsonSerializer.Deserialize<T>(payload, SerializerOptions);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(value, SerializerOptions);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromSeconds(options.Value.DefaultExpirationSeconds)
        };

        return distributedCache.SetStringAsync(key, payload, cacheOptions, cancellationToken);
    }

    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T?>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var created = await factory(cancellationToken);
        if (created is not null)
        {
            await SetAsync(key, created, expiration, cancellationToken);
        }

        return created;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return distributedCache.RemoveAsync(key, cancellationToken);
    }
}
