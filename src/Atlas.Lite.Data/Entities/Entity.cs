namespace Atlas.Lite.Data.Entities;

public abstract class Entity<TKey> : IEntity<TKey>
{
    public TKey Id { get; set; } = default!;
}
