using Atlas.Lite.Data.Repositories;
using Atlas.Lite.Data.Tests.Support;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Lite.Data.Tests;

public sealed class AppDbContextConventionTests
{
    [Fact]
    public async Task SaveChangesAsync_AppliesAuditFields()
    {
        var now = new DateTimeOffset(2026, 5, 24, 12, 0, 0, TimeSpan.Zero);
        await using var dbContext = CreateDbContext(now);
        var repository = new EfRepository<TestEntity, Guid>(dbContext);
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "First" };

        await repository.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        Assert.Equal(now, entity.CreatedAt);
        Assert.Equal(now, entity.ModifiedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_ConvertsDeleteToSoftDelete()
    {
        var now = new DateTimeOffset(2026, 5, 24, 12, 0, 0, TimeSpan.Zero);
        await using var dbContext = CreateDbContext(now);
        var repository = new EfRepository<TestEntity, Guid>(dbContext);
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "First" };

        await repository.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        repository.Remove(entity);
        await dbContext.SaveChangesAsync();

        Assert.True(entity.IsDeleted);
        Assert.Equal(now, entity.DeletedAt);
        Assert.Empty(await dbContext.Entities.ToListAsync());
        Assert.Single(await dbContext.Entities.IgnoreQueryFilters().ToListAsync());
    }

    private static TestDbContext CreateDbContext(DateTimeOffset now)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new TestDbContext(options, new FixedClock(now));
    }
}
