namespace Atlas.Lite.Data.Entities;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; set; }

    DateTimeOffset? ModifiedAt { get; set; }
}
