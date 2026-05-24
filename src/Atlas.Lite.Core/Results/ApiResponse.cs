namespace Atlas.Lite.Core.Results;

public sealed record ApiResponse<T>(bool Success, T? Data, IReadOnlyList<Error> Errors)
{
    public static ApiResponse<T> Ok(T data) => new(true, data, Array.Empty<Error>());

    public static ApiResponse<T> Fail(params Error[] errors) => new(false, default, errors);
}
