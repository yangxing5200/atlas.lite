namespace Atlas.Lite.Core.Pagination;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    long TotalCount)
{
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static PagedResult<T> Create(
        IReadOnlyList<T> items,
        PagedRequest request,
        long totalCount)
    {
        return new PagedResult<T>(
            items,
            request.NormalizedPageNumber,
            request.NormalizedPageSize,
            totalCount);
    }
}
