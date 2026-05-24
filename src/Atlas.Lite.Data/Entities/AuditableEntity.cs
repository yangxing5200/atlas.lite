namespace Atlas.Lite.Data.Entities;

public abstract class AuditableEntity<TKey> : Entity<TKey>, IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }
}
