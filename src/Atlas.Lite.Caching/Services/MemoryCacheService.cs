using Atlas.Lite.Caching.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Atlas.Lite.Caching.Services;

public sealed class MemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<AtlasLiteCacheOptions> options) : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(memoryCache.TryGetValue<T>(key, out var value) ? value : default);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        memoryCache.Set(key, value, expiration ?? TimeSpan.FromSeconds(options.Value.DefaultExpirationSeconds));
        return Task.CompletedTask;
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
        cancellationToken.ThrowIfCancellationRequested();
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}
