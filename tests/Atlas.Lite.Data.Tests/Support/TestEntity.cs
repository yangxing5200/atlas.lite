using Atlas.Lite.Data.Entities;

namespace Atlas.Lite.Data.Tests.Support;

internal sealed class TestEntity : AuditableSoftDeleteEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
}
