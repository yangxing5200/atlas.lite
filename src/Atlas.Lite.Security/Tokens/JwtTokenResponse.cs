namespace Atlas.Lite.Security.Tokens;

public sealed record JwtTokenResponse(string AccessToken, DateTimeOffset ExpiresAt);
