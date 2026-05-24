using Microsoft.Extensions.Options;

namespace Atlas.Lite.Data.Options;

public sealed class AtlasLiteDatabaseOptionsValidator : IValidateOptions<AtlasLiteDatabaseOptions>
{
    public ValidateOptionsResult Validate(string? name, AtlasLiteDatabaseOptions options)
    {
        var failures = new List<string>();

        if (!IsSupportedProvider(options.Provider))
        {
            failures.Add("AtlasLite:Database:Provider must be Sqlite, SqlServer, or InMemory.");
        }

        if (!IsInMemory(options.Provider) && string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            failures.Add("AtlasLite:Database:ConnectionString is required when the provider is not InMemory.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static bool IsSupportedProvider(string provider)
    {
        return IsProvider(provider, DatabaseProviders.Sqlite) ||
            IsProvider(provider, DatabaseProviders.SqlServer) ||
            IsInMemory(provider);
    }

    private static bool IsInMemory(string provider) => IsProvider(provider, DatabaseProviders.InMemory);

    private static bool IsProvider(string provider, string expected)
    {
        return string.Equals(provider, expected, StringComparison.OrdinalIgnoreCase);
    }
}
