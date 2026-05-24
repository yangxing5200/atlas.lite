using Atlas.Lite.Core.Abstractions;
using Atlas.Lite.Data.Options;
using Atlas.Lite.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Atlas.Lite.Data.DependencyInjection;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasLiteData<TContext>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TContext : DbContext
    {
        services
            .AddOptions<AtlasLiteDatabaseOptions>()
            .Bind(configuration.GetSection(AtlasLiteDatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<AtlasLiteDatabaseOptions>, AtlasLiteDatabaseOptionsValidator>();
        services.TryAddSingleton<IClock, SystemClock>();
        services.TryAddSingleton<IIdGenerator, GuidIdGenerator>();
        services.AddDbContext<TContext>((_, builder) => ConfigureProvider(builder, configuration));
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TContext>());
        services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        ValidateOptions(configuration);

        return services;
    }

    private static void ConfigureProvider(DbContextOptionsBuilder builder, IConfiguration configuration)
    {
        var options = configuration
            .GetSection(AtlasLiteDatabaseOptions.SectionName)
            .Get<AtlasLiteDatabaseOptions>() ?? new AtlasLiteDatabaseOptions();

        if (string.Equals(options.Provider, DatabaseProviders.Sqlite, StringComparison.OrdinalIgnoreCase))
        {
            builder.UseSqlite(options.ConnectionString);
            return;
        }

        if (string.Equals(options.Provider, DatabaseProviders.SqlServer, StringComparison.OrdinalIgnoreCase))
        {
            builder.UseSqlServer(options.ConnectionString, sql => sql.EnableRetryOnFailure());
            return;
        }

        if (string.Equals(options.Provider, DatabaseProviders.InMemory, StringComparison.OrdinalIgnoreCase))
        {
            builder.UseInMemoryDatabase(string.IsNullOrWhiteSpace(options.ConnectionString)
                ? "AtlasLite"
                : options.ConnectionString);
            return;
        }

        throw new InvalidOperationException($"Unsupported database provider '{options.Provider}'.");
    }

    private static void ValidateOptions(IConfiguration configuration)
    {
        var options = configuration
            .GetSection(AtlasLiteDatabaseOptions.SectionName)
            .Get<AtlasLiteDatabaseOptions>() ?? new AtlasLiteDatabaseOptions();

        var validation = new AtlasLiteDatabaseOptionsValidator().Validate(Microsoft.Extensions.Options.Options.DefaultName, options);
        if (validation.Failed)
        {
            throw new OptionsValidationException(
                Microsoft.Extensions.Options.Options.DefaultName,
                typeof(AtlasLiteDatabaseOptions),
                validation.Failures);
        }
    }
}
