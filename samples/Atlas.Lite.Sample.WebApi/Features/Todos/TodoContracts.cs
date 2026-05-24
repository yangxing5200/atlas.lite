namespace Atlas.Lite.Sample.WebApi.Features.Todos;

public sealed record CreateTodoRequest(string Title);

public sealed record UpdateTodoRequest(string Title, bool IsCompleted);

public sealed record TodoResponse(
    Guid Id,
    string Title,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ModifiedAt)
{
    public static TodoResponse FromEntity(TodoItem item)
    {
        return new TodoResponse(
            item.Id,
            item.Title,
            item.IsCompleted,
            item.CreatedAt,
            item.ModifiedAt);
    }
}
