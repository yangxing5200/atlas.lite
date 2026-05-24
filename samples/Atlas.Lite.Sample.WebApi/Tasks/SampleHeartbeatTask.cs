using Atlas.Lite.BackgroundTasks.Services;

namespace Atlas.Lite.Sample.WebApi.Tasks;

public sealed class SampleHeartbeatTask(ILogger<SampleHeartbeatTask> logger) : IAtlasLiteBackgroundTask
{
    public string Name => "sample-heartbeat";

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Sample heartbeat task executed at {UtcNow}.", DateTimeOffset.UtcNow);
        return Task.CompletedTask;
    }
}
