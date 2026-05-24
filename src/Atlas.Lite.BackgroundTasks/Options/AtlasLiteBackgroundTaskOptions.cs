using System.ComponentModel.DataAnnotations;

namespace Atlas.Lite.BackgroundTasks.Options;

public sealed class AtlasLiteBackgroundTaskOptions
{
    public const string SectionName = "AtlasLite:BackgroundTasks";

    public bool Enabled { get; set; } = true;

    [Range(1, 86400)]
    public int IntervalSeconds { get; set; } = 60;
}
