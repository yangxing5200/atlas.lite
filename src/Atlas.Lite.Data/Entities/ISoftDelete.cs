namespace Atlas.Lite.Data.Entities;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }

    DateTimeOffset? DeletedAt { get; set; }
}
