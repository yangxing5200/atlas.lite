namespace Atlas.Lite.Core.Abstractions;

public sealed class GuidIdGenerator : IIdGenerator
{
    public Guid NewGuid() => Guid.NewGuid();
}
