namespace Atlas.Lite.Infrastructure.Logging.Sanitizing;

public interface ILogSanitizer
{
    object? Sanitize(string fieldName, object? value);
}
