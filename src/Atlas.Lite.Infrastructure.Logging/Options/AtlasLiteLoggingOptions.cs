using System.ComponentModel.DataAnnotations;

namespace Atlas.Lite.Infrastructure.Logging.Options;

public sealed class AtlasLiteLoggingOptions
{
    public const string SectionName = "AtlasLite:Logging";

    [Required]
    public string MinimumLevel { get; set; } = "Information";

    public ConsoleLoggingOptions Console { get; set; } = new();

    public FileLoggingOptions File { get; set; } = new();

    public SeqLoggingOptions Seq { get; set; } = new();

    public string[] SensitiveFields { get; set; } =
    [
        "password",
        "secret",
        "token",
        "authorization"
    ];
}

public sealed class ConsoleLoggingOptions
{
    public bool Enabled { get; set; } = true;
}

public sealed class FileLoggingOptions
{
    public bool Enabled { get; set; }

    public string? Path { get; set; } = "logs/atlas-lite-.log";
}

public sealed class SeqLoggingOptions
{
    public bool Enabled { get; set; }

    public string? ServerUrl { get; set; }
}
