using Atlas.Lite.BackgroundTasks.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Atlas.Lite.BackgroundTasks.Services;

public sealed class AtlasLiteBackgroundTaskService(
    IEnumerable<IAtlasLiteBackgroundTask> tasks,
    IOptions<AtlasLiteBackgroundTaskOptions> options,
    ILogger<AtlasLiteBackgroundTaskService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var taskList = tasks.ToArray();
        if (!options.Value.Enabled)
        {
            logger.LogInformation("Atlas.Lite background tasks are disabled.");
            return;
        }

        if (taskList.Length == 0)
        {
            logger.LogInformation("No Atlas.Lite background tasks are registered.");
            return;
        }

        var interval = TimeSpan.FromSeconds(options.Value.IntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var task in taskList)
            {
                try
                {
                    logger.LogDebug("Executing background task {TaskName}.", task.Name);
                    await task.ExecuteAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Background task {TaskName} failed.", task.Name);
                }
            }

            await Task.Delay(interval, stoppingToken);
        }
    }
}
