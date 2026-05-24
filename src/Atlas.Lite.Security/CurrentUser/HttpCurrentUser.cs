using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Atlas.Lite.Security.CurrentUser;

public sealed class HttpCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? UserName => User?.Identity?.Name ?? User?.FindFirst(ClaimTypes.Name)?.Value;

    public IReadOnlyCollection<string> Roles => User?
        .FindAll(ClaimTypes.Role)
        .Select(claim => claim.Value)
        .ToArray() ?? Array.Empty<string>();

    public bool IsInRole(string role) => User?.IsInRole(role) == true;
}
