namespace Atlas.Lite.Data.Entities;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
