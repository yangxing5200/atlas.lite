namespace Atlas.Lite.Core.Pagination;

public sealed record PagedRequest(int PageNumber = 1, int PageSize = 20)
{
    public const int MaxPageSize = 200;

    public int Skip => (NormalizedPageNumber - 1) * NormalizedPageSize;

    public int NormalizedPageNumber => Math.Max(1, PageNumber);

    public int NormalizedPageSize => Math.Clamp(PageSize, 1, MaxPageSize);
}
