using Atlas.Lite.Web.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Atlas.Lite.Web.DependencyInjection;

public static class WebApplicationExtensions
{
    public static WebApplication UseAtlasLiteWebApi(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<AtlasLiteWebOptions>>().Value;

        app.UseExceptionHandler();
        app.UseCors(WebApiServiceCollectionExtensions.CorsPolicyName);

        if (app.Environment.IsDevelopment() && options.EnableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapHealthChecks("/health");

        return app;
    }
}
