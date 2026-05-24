using Atlas.Lite.Core.Abstractions;

namespace Atlas.Lite.Data.Tests.Support;

internal sealed class FixedClock(DateTimeOffset utcNow) : IClock
{
    public DateTimeOffset UtcNow { get; } = utcNow;
}
