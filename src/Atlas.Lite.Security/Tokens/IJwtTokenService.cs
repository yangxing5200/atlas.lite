namespace Atlas.Lite.Security.Tokens;

public interface IJwtTokenService
{
    JwtTokenResponse CreateToken(JwtTokenRequest request);
}
