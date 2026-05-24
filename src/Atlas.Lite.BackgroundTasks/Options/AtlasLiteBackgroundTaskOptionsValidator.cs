using Microsoft.Extensions.Options;

namespace Atlas.Lite.BackgroundTasks.Options;

public sealed class AtlasLiteBackgroundTaskOptionsValidator : IValidateOptions<AtlasLiteBackgroundTaskOptions>
{
    public ValidateOptionsResult Validate(string? name, AtlasLiteBackgroundTaskOptions options)
    {
        if (options.IntervalSeconds is < 1 or > 86400)
        {
            return ValidateOptionsResult.Fail("AtlasLite:BackgroundTasks:IntervalSeconds must be between 1 and 86400.");
        }

        return ValidateOptionsResult.Success;
    }
}
