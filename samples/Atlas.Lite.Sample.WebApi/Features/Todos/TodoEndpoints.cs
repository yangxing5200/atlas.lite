using Atlas.Lite.Caching.Services;
using Atlas.Lite.Core.Abstractions;
using Atlas.Lite.Core.Pagination;
using Atlas.Lite.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Lite.Sample.WebApi.Features.Todos;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/todos")
            .WithTags("Todos")
            .RequireAuthorization();

        group.MapGet("/", async (
            IRepository<TodoItem, Guid> repository,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken) =>
        {
            var request = new PagedRequest(pageNumber, pageSize);
            var query = repository.Query().OrderByDescending(todo => todo.CreatedAt);
            var totalCount = await query.LongCountAsync(cancellationToken);
            var entities = await query
                .Skip(request.Skip)
                .Take(request.NormalizedPageSize)
                .ToListAsync(cancellationToken);
            var items = entities.Select(TodoResponse.FromEntity).ToList();

            return Results.Ok(PagedResult<TodoResponse>.Create(items, request, totalCount));
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IRepository<TodoItem, Guid> repository,
            ICacheService cache,
            CancellationToken cancellationToken) =>
        {
            var cacheKey = CacheKey.Build("todos", id.ToString("N"));
            var response = await cache.GetOrCreateAsync(
                cacheKey,
                async token =>
                {
                    var item = await repository.GetByIdAsync(id, token);
                    return item is null ? null : TodoResponse.FromEntity(item);
                },
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return response is null ? (IResult)Results.NotFound() : Results.Ok(response);
        });

        group.MapPost("/", async (
            CreateTodoRequest request,
            IRepository<TodoItem, Guid> repository,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return (IResult)Results.BadRequest(new { error = "Title is required." });
            }

            var item = new TodoItem
            {
                Id = idGenerator.NewGuid(),
                Title = request.Title.Trim()
            };

            await repository.AddAsync(item, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/todos/{item.Id}", TodoResponse.FromEntity(item));
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateTodoRequest request,
            IRepository<TodoItem, Guid> repository,
            IUnitOfWork unitOfWork,
            ICacheService cache,
            CancellationToken cancellationToken) =>
        {
            var item = await repository.GetByIdAsync(id, cancellationToken);
            if (item is null)
            {
                return (IResult)Results.NotFound();
            }

            item.Title = request.Title.Trim();
            item.IsCompleted = request.IsCompleted;
            repository.Update(item);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await cache.RemoveAsync(CacheKey.Build("todos", id.ToString("N")), cancellationToken);

            return Results.Ok(TodoResponse.FromEntity(item));
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IRepository<TodoItem, Guid> repository,
            IUnitOfWork unitOfWork,
            ICacheService cache,
            CancellationToken cancellationToken) =>
        {
            var item = await repository.GetByIdAsync(id, cancellationToken);
            if (item is null)
            {
                return (IResult)Results.NotFound();
            }

            repository.Remove(item);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await cache.RemoveAsync(CacheKey.Build("todos", id.ToString("N")), cancellationToken);

            return Results.NoContent();
        });

        return endpoints;
    }
}
