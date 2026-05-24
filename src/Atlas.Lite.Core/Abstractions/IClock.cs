namespace Atlas.Lite.Core.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
