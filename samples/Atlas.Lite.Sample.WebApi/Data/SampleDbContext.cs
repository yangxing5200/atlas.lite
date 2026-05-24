using Atlas.Lite.Core.Abstractions;
using Atlas.Lite.Data;
using Atlas.Lite.Sample.WebApi.Features.Todos;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Lite.Sample.WebApi.Data;

public sealed class SampleDbContext(
    DbContextOptions<SampleDbContext> options,
    IClock clock) : AppDbContext(options, clock)
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(todo => todo.Id);
            entity.Property(todo => todo.Title).HasMaxLength(200).IsRequired();
        });
    }
}
