namespace Atlas.Lite.Security.CurrentUser;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    string? UserId { get; }

    string? UserName { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsInRole(string role);
}
