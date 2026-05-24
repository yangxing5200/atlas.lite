namespace Atlas.Lite.Data.Entities;

public abstract class AuditableSoftDeleteEntity<TKey> : AuditableEntity<TKey>, ISoftDelete
{
    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}
