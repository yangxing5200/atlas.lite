using Atlas.Lite.Security.CurrentUser;
using Atlas.Lite.Security.Tokens;

namespace Atlas.Lite.Sample.WebApi.Features.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", (LoginRequest request, IJwtTokenService tokenService) =>
        {
            if (!DemoUserCredentials.IsValid(request.UserName, request.Password))
            {
                return (IResult)Results.Unauthorized();
            }

            var token = tokenService.CreateToken(new JwtTokenRequest(
                Subject: DemoUserCredentials.UserId,
                UserName: request.UserName,
                Roles: ["Admin"]));

            return Results.Ok(token);
        }).AllowAnonymous();

        group.MapGet("/me", (ICurrentUser currentUser) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return (IResult)Results.Unauthorized();
            }

            return Results.Ok(new CurrentUserResponse(
                currentUser.UserId,
                currentUser.UserName,
                currentUser.Roles));
        }).RequireAuthorization();

        return endpoints;
    }
}

public sealed record LoginRequest(string UserName, string Password);

public sealed record CurrentUserResponse(
    string? UserId,
    string? UserName,
    IReadOnlyCollection<string> Roles);

internal static class DemoUserCredentials
{
    public const string UserId = "demo-user";

    public static bool IsValid(string userName, string password)
    {
        return string.Equals(userName, "demo", StringComparison.OrdinalIgnoreCase) &&
            password == "demo-password";
    }
}
