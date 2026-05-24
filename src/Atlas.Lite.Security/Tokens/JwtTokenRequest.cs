namespace Atlas.Lite.Security.Tokens;

public sealed record JwtTokenRequest(
    string Subject,
    string UserName,
    IReadOnlyCollection<string>? Roles = null,
    IReadOnlyDictionary<string, string>? Claims = null);
