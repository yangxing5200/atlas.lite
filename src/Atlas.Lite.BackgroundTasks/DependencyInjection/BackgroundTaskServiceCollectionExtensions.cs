using Atlas.Lite.BackgroundTasks.Options;
using Atlas.Lite.BackgroundTasks.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Atlas.Lite.BackgroundTasks.DependencyInjection;

public static class BackgroundTaskServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasLiteBackgroundTasks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AtlasLiteBackgroundTaskOptions>()
            .Bind(configuration.GetSection(AtlasLiteBackgroundTaskOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<AtlasLiteBackgroundTaskOptions>, AtlasLiteBackgroundTaskOptionsValidator>();

        var options = configuration
            .GetSection(AtlasLiteBackgroundTaskOptions.SectionName)
            .Get<AtlasLiteBackgroundTaskOptions>() ?? new AtlasLiteBackgroundTaskOptions();

        var validation = new AtlasLiteBackgroundTaskOptionsValidator().Validate(Microsoft.Extensions.Options.Options.DefaultName, options);
        if (validation.Failed)
        {
            throw new OptionsValidationException(
                Microsoft.Extensions.Options.Options.DefaultName,
                typeof(AtlasLiteBackgroundTaskOptions),
                validation.Failures);
        }

        if (options.Enabled)
        {
            services.AddHostedService<AtlasLiteBackgroundTaskService>();
        }

        return services;
    }

    public static IServiceCollection AddAtlasLiteBackgroundTask<TTask>(this IServiceCollection services)
        where TTask : class, IAtlasLiteBackgroundTask
    {
        services.AddSingleton<IAtlasLiteBackgroundTask, TTask>();
        return services;
    }
}
