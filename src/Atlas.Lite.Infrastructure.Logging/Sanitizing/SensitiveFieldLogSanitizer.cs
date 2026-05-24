using Atlas.Lite.Infrastructure.Logging.Options;
using Microsoft.Extensions.Options;

namespace Atlas.Lite.Infrastructure.Logging.Sanitizing;

public sealed class SensitiveFieldLogSanitizer : ILogSanitizer
{
    private const string Mask = "***";
    private readonly HashSet<string> sensitiveFields;

    public SensitiveFieldLogSanitizer(IOptions<AtlasLiteLoggingOptions> options)
    {
        sensitiveFields = options.Value.SensitiveFields
            .Select(field => field.Trim())
            .Where(field => field.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public object? Sanitize(string fieldName, object? value)
    {
        return sensitiveFields.Contains(fieldName) ? Mask : value;
    }
}
