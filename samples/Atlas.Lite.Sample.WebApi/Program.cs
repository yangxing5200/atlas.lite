using Atlas.Lite.BackgroundTasks.DependencyInjection;
using Atlas.Lite.Caching.DependencyInjection;
using Atlas.Lite.Data.DependencyInjection;
using Atlas.Lite.Infrastructure.Logging.DependencyInjection;
using Atlas.Lite.Sample.WebApi.Data;
using Atlas.Lite.Sample.WebApi.Features.Auth;
using Atlas.Lite.Sample.WebApi.Features.Todos;
using Atlas.Lite.Sample.WebApi.Tasks;
using Atlas.Lite.Security.DependencyInjection;
using Atlas.Lite.Web.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAtlasLiteSerilog(builder.Configuration);

builder.Services.AddAtlasLiteLogging(builder.Configuration);
builder.Services.AddAtlasLiteWebApi(builder.Configuration);
builder.Services.AddAtlasLiteData<SampleDbContext>(builder.Configuration);
builder.Services.AddAtlasLiteSecurity(builder.Configuration);
builder.Services.AddAtlasLiteCaching(builder.Configuration);
builder.Services.AddAtlasLiteBackgroundTask<SampleHeartbeatTask>();
builder.Services.AddAtlasLiteBackgroundTasks(builder.Configuration);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseAtlasLiteWebApi();
app.UseAtlasLiteSecurity();

app.MapAuthEndpoints();
app.MapTodoEndpoints();

await app.RunAsync();
