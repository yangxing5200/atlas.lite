using Microsoft.EntityFrameworkCore;

namespace Atlas.Lite.Data.Repositories;

public sealed class EfUnitOfWork(DbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
