using System.Linq.Expressions;
using System.Reflection;
using Atlas.Lite.Core.Abstractions;
using Atlas.Lite.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Lite.Data;

public abstract class AppDbContext(DbContextOptions options, IClock clock) : DbContext(options)
{
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyConventions();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyConventions();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    private void ApplyConventions()
    {
        var now = clock.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete softDelete)
            {
                entry.State = EntityState.Modified;
                softDelete.IsDeleted = true;
                softDelete.DeletedAt = now;
            }

            if (entry.Entity is not IAuditableEntity auditable)
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                if (auditable.CreatedAt == default)
                {
                    auditable.CreatedAt = now;
                }
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                auditable.ModifiedAt = now;
            }
        }
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var propertyMethod = typeof(EF)
                .GetMethod(nameof(EF.Property), BindingFlags.Public | BindingFlags.Static)!
                .MakeGenericMethod(typeof(bool));
            var isDeletedProperty = Expression.Call(
                propertyMethod,
                parameter,
                Expression.Constant(nameof(ISoftDelete.IsDeleted)));
            var filter = Expression.Lambda(
                Expression.Equal(isDeletedProperty, Expression.Constant(false)),
                parameter);

            entityType.SetQueryFilter(filter);
        }
    }
}
