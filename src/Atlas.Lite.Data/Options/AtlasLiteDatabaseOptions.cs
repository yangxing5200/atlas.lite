using System.ComponentModel.DataAnnotations;

namespace Atlas.Lite.Data.Options;

public sealed class AtlasLiteDatabaseOptions
{
    public const string SectionName = "AtlasLite:Database";

    [Required]
    public string Provider { get; set; } = DatabaseProviders.Sqlite;

    public string? ConnectionString { get; set; }
}

public static class DatabaseProviders
{
    public const string Sqlite = "Sqlite";
    public const string SqlServer = "SqlServer";
    public const string InMemory = "InMemory";
}
