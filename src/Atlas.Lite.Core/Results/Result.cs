namespace Atlas.Lite.Core.Results;

public sealed record Result(bool Succeeded, IReadOnlyList<Error> Errors)
{
    public bool Failed => !Succeeded;

    public static Result Success() => new(true, Array.Empty<Error>());

    public static Result Failure(params Error[] errors) => new(false, errors);

    public static Result Failure(IEnumerable<Error> errors) => new(false, errors.ToArray());
}
