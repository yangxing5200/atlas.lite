using Atlas.Lite.Core.Pagination;

namespace Atlas.Lite.Core.Tests;

public sealed class PaginationTests
{
    [Fact]
    public void PagedRequest_NormalizesInvalidValues()
    {
        var request = new PagedRequest(PageNumber: -10, PageSize: 500);

        Assert.Equal(1, request.NormalizedPageNumber);
        Assert.Equal(PagedRequest.MaxPageSize, request.NormalizedPageSize);
        Assert.Equal(0, request.Skip);
    }

    [Fact]
    public void PagedResult_ComputesNavigationFlags()
    {
        var result = PagedResult<int>.Create([1, 2], new PagedRequest(2, 2), totalCount: 5);

        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }
}
