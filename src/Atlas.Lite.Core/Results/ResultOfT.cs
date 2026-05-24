namespace Atlas.Lite.Core.Results;

public sealed record Result<T>(bool Succeeded, T? Value, IReadOnlyList<Error> Errors)
{
    public bool Failed => !Succeeded;

    public static Result<T> Success(T value) => new(true, value, Array.Empty<Error>());

    public static Result<T> Failure(params Error[] errors) => new(false, default, errors);

    public static Result<T> Failure(IEnumerable<Error> errors) => new(false, default, errors.ToArray());
}
