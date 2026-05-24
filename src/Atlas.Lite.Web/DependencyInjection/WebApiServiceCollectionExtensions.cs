using Atlas.Lite.Web.ExceptionHandling;
using Atlas.Lite.Web.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Atlas.Lite.Web.DependencyInjection;

public static class WebApiServiceCollectionExtensions
{
    public const string CorsPolicyName = "AtlasLiteCors";

    public static IServiceCollection AddAtlasLiteWebApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AtlasLiteWebOptions>()
            .Bind(configuration.GetSection(AtlasLiteWebOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<AtlasLiteWebOptions>, AtlasLiteWebOptionsValidator>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();

        var webOptions = configuration
            .GetSection(AtlasLiteWebOptions.SectionName)
            .Get<AtlasLiteWebOptions>() ?? new AtlasLiteWebOptions();

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                policy.AllowAnyHeader().AllowAnyMethod();

                if (webOptions.AllowedOrigins.Length == 0)
                {
                    policy.AllowAnyOrigin();
                }
                else
                {
                    policy.WithOrigins(webOptions.AllowedOrigins);
                }
            });
        });

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.WriteIndented = false;
        });

        return services;
    }
}
