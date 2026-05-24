using System.ComponentModel.DataAnnotations;

namespace Atlas.Lite.Web.Options;

public sealed class AtlasLiteWebOptions
{
    public const string SectionName = "AtlasLite:Web";

    [Required]
    public string ApplicationName { get; set; } = "Atlas.Lite";

    public bool EnableSwagger { get; set; } = true;

    public string[] AllowedOrigins { get; set; } = [];
}
