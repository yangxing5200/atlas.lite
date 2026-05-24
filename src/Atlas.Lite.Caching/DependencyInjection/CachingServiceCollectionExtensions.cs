using Atlas.Lite.Caching.Options;
using Atlas.Lite.Caching.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Atlas.Lite.Caching.DependencyInjection;

public static class CachingServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasLiteCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AtlasLiteCacheOptions>()
            .Bind(configuration.GetSection(AtlasLiteCacheOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<AtlasLiteCacheOptions>, AtlasLiteCacheOptionsValidator>();

        var options = ValidateOptions(configuration);
        if (string.Equals(options.Provider, CacheProviders.Redis, StringComparison.OrdinalIgnoreCase))
        {
            services.AddStackExchangeRedisCache(redis =>
            {
                redis.Configuration = options.Redis.ConnectionString;
            });
            services.AddSingleton<ICacheService, DistributedCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
        }

        return services;
    }

    private static AtlasLiteCacheOptions ValidateOptions(IConfiguration configuration)
    {
        var options = configuration
            .GetSection(AtlasLiteCacheOptions.SectionName)
            .Get<AtlasLiteCacheOptions>() ?? new AtlasLiteCacheOptions();

        var validation = new AtlasLiteCacheOptionsValidator().Validate(Microsoft.Extensions.Options.Options.DefaultName, options);
        if (validation.Failed)
        {
            throw new OptionsValidationException(
                Microsoft.Extensions.Options.Options.DefaultName,
                typeof(AtlasLiteCacheOptions),
                validation.Failures);
        }

        return options;
    }
}
