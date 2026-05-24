using Atlas.Lite.Infrastructure.Logging.Options;
using Atlas.Lite.Infrastructure.Logging.Sanitizing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace Atlas.Lite.Infrastructure.Logging.DependencyInjection;

public static class LoggingServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasLiteLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AtlasLiteLoggingOptions>()
            .Bind(configuration.GetSection(AtlasLiteLoggingOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<AtlasLiteLoggingOptions>, AtlasLiteLoggingOptionsValidator>();
        services.AddSingleton<ILogSanitizer, SensitiveFieldLogSanitizer>();

        return services;
    }

    public static IHostBuilder UseAtlasLiteSerilog(
        this IHostBuilder hostBuilder,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(AtlasLiteLoggingOptions.SectionName)
            .Get<AtlasLiteLoggingOptions>() ?? new AtlasLiteLoggingOptions();

        var validation = new AtlasLiteLoggingOptionsValidator().Validate(Microsoft.Extensions.Options.Options.DefaultName, options);
        if (validation.Failed)
        {
            throw new OptionsValidationException(
                Microsoft.Extensions.Options.Options.DefaultName,
                typeof(AtlasLiteLoggingOptions),
                validation.Failures);
        }

        return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            var level = Enum.Parse<LogEventLevel>(options.MinimumLevel, ignoreCase: true);

            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .MinimumLevel.Is(level)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

            if (options.Console.Enabled)
            {
                loggerConfiguration.WriteTo.Console();
            }

            if (options.File.Enabled)
            {
                loggerConfiguration.WriteTo.File(
                    options.File.Path!,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14);
            }

            if (options.Seq.Enabled)
            {
                loggerConfiguration.WriteTo.Seq(options.Seq.ServerUrl!);
            }
        });
    }
}
