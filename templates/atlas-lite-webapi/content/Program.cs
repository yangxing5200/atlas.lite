namespace AtlasLiteTemplateNamespace;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.MapHealthChecks("/health");
        app.MapGet("/", () => Results.Ok(new
        {
            application = "AtlasLiteTemplate",
            status = "running"
        }));

        app.Run();
    }
}
