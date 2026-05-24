namespace Atlas.Lite.BackgroundTasks.Services;

public interface IAtlasLiteBackgroundTask
{
    string Name { get; }

    Task ExecuteAsync(CancellationToken cancellationToken);
}
