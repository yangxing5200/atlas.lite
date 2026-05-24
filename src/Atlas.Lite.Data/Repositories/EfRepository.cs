using Atlas.Lite.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Lite.Data.Repositories;

public sealed class EfRepository<TEntity, TKey>(DbContext dbContext) : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    private readonly DbSet<TEntity> dbSet = dbContext.Set<TEntity>();

    public IQueryable<TEntity> Query() => dbSet;

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await dbSet.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        dbSet.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        dbSet.Remove(entity);
    }

    public async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(id, cancellationToken) is not null;
    }
}
