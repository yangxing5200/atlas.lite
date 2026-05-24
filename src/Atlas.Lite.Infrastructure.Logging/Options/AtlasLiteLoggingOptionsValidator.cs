using Microsoft.Extensions.Options;
using Serilog.Events;

namespace Atlas.Lite.Infrastructure.Logging.Options;

public sealed class AtlasLiteLoggingOptionsValidator : IValidateOptions<AtlasLiteLoggingOptions>
{
    public ValidateOptionsResult Validate(string? name, AtlasLiteLoggingOptions options)
    {
        var failures = new List<string>();

        if (!Enum.TryParse<LogEventLevel>(options.MinimumLevel, ignoreCase: true, out _))
        {
            failures.Add("AtlasLite:Logging:MinimumLevel must be one of Verbose, Debug, Information, Warning, Error, or Fatal.");
        }

        if (!options.Console.Enabled && !options.File.Enabled && !options.Seq.Enabled)
        {
            failures.Add("At least one logging sink must be enabled.");
        }

        if (options.File.Enabled && string.IsNullOrWhiteSpace(options.File.Path))
        {
            failures.Add("AtlasLite:Logging:File:Path is required when file logging is enabled.");
        }

        if (options.Seq.Enabled)
        {
            if (string.IsNullOrWhiteSpace(options.Seq.ServerUrl))
            {
                failures.Add("AtlasLite:Logging:Seq:ServerUrl is required when Seq logging is enabled.");
            }
            else if (!Uri.TryCreate(options.Seq.ServerUrl, UriKind.Absolute, out _))
            {
                failures.Add("AtlasLite:Logging:Seq:ServerUrl must be an absolute URL.");
            }
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
