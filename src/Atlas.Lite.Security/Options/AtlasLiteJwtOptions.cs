using System.ComponentModel.DataAnnotations;

namespace Atlas.Lite.Security.Options;

public sealed class AtlasLiteJwtOptions
{
    public const string SectionName = "AtlasLite:Auth:Jwt";

    [Required]
    public string Issuer { get; set; } = "Atlas.Lite";

    [Required]
    public string Audience { get; set; } = "Atlas.Lite";

    [Required]
    public string Secret { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 60;

    public int ClockSkewSeconds { get; set; } = 30;
}
