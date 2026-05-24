using Atlas.Lite.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Lite.Data.Tests.Support;

internal sealed class TestDbContext(
    DbContextOptions<TestDbContext> options,
    IClock clock) : AppDbContext(options, clock)
{
    public DbSet<TestEntity> Entities => Set<TestEntity>();
}
