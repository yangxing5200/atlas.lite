namespace Atlas.Lite.Core.Exceptions;

public sealed class ValidationException : AtlasLiteException
{
    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("validation_failed", "One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
