using Microsoft.Extensions.Options;

namespace Atlas.Lite.Security.Options;

public sealed class AtlasLiteJwtOptionsValidator : IValidateOptions<AtlasLiteJwtOptions>
{
    public ValidateOptionsResult Validate(string? name, AtlasLiteJwtOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            failures.Add("AtlasLite:Auth:Jwt:Issuer is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            failures.Add("AtlasLite:Auth:Jwt:Audience is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Secret) || options.Secret.Length < 32)
        {
            failures.Add("AtlasLite:Auth:Jwt:Secret must be at least 32 characters long.");
        }

        if (options.ExpirationMinutes is < 1 or > 1440)
        {
            failures.Add("AtlasLite:Auth:Jwt:ExpirationMinutes must be between 1 and 1440.");
        }

        if (options.ClockSkewSeconds is < 0 or > 300)
        {
            failures.Add("AtlasLite:Auth:Jwt:ClockSkewSeconds must be between 0 and 300.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
