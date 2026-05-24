using Atlas.Lite.Data.Entities;

namespace Atlas.Lite.Data.Repositories;

public interface IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    IQueryable<TEntity> Query();

    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
}
