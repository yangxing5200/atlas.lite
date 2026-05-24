using Microsoft.Extensions.Options;

namespace Atlas.Lite.Web.Options;

public sealed class AtlasLiteWebOptionsValidator : IValidateOptions<AtlasLiteWebOptions>
{
    public ValidateOptionsResult Validate(string? name, AtlasLiteWebOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ApplicationName))
        {
            failures.Add("AtlasLite:Web:ApplicationName is required.");
        }

        foreach (var origin in options.AllowedOrigins.Where(origin => !string.IsNullOrWhiteSpace(origin)))
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                failures.Add($"AtlasLite:Web:AllowedOrigins contains an invalid HTTP origin: '{origin}'.");
            }
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
