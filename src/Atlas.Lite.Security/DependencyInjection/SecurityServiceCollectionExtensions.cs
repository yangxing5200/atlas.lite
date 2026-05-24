using System.Text;
using Atlas.Lite.Core.Abstractions;
using Atlas.Lite.Security.CurrentUser;
using Atlas.Lite.Security.Options;
using Atlas.Lite.Security.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Atlas.Lite.Security.DependencyInjection;

public static class SecurityServiceCollectionExtensions
{
    public const string AuthenticatedPolicy = "AtlasLite.Authenticated";

    public static IServiceCollection AddAtlasLiteSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AtlasLiteJwtOptions>()
            .Bind(configuration.GetSection(AtlasLiteJwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<AtlasLiteJwtOptions>, AtlasLiteJwtOptionsValidator>();

        var jwtOptions = ValidateOptions(configuration);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));

        services.TryAddSingleton<IClock, SystemClock>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpCurrentUser>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkewSeconds)
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticatedPolicy, policy => policy.RequireAuthenticatedUser());

        return services;
    }

    public static IApplicationBuilder UseAtlasLiteSecurity(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    private static AtlasLiteJwtOptions ValidateOptions(IConfiguration configuration)
    {
        var options = configuration
            .GetSection(AtlasLiteJwtOptions.SectionName)
            .Get<AtlasLiteJwtOptions>() ?? new AtlasLiteJwtOptions();

        var validation = new AtlasLiteJwtOptionsValidator().Validate(Microsoft.Extensions.Options.Options.DefaultName, options);
        if (validation.Failed)
        {
            throw new OptionsValidationException(
                Microsoft.Extensions.Options.Options.DefaultName,
                typeof(AtlasLiteJwtOptions),
                validation.Failures);
        }

        return options;
    }
}
