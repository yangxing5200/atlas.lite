using Atlas.Lite.Data.Entities;

namespace Atlas.Lite.Sample.WebApi.Features.Todos;

public sealed class TodoItem : AuditableSoftDeleteEntity<Guid>
{
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
