using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Atlas.Lite.Core.Abstractions;
using Atlas.Lite.Security.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Atlas.Lite.Security.Tokens;

public sealed class JwtTokenService(
    IOptions<AtlasLiteJwtOptions> options,
    IClock clock) : IJwtTokenService
{
    public JwtTokenResponse CreateToken(JwtTokenRequest request)
    {
        var jwtOptions = options.Value;
        var now = clock.UtcNow;
        var expiresAt = now.AddMinutes(jwtOptions.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.Subject),
            new(ClaimTypes.NameIdentifier, request.Subject),
            new(ClaimTypes.Name, request.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        foreach (var role in request.Roles ?? Array.Empty<string>())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (request.Claims is not null)
        {
            foreach (var claim in request.Claims)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new JwtTokenResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
