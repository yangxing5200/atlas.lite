using Atlas.Lite.Core.Exceptions;
using Atlas.Lite.Web.ExceptionHandling;
using Microsoft.AspNetCore.Http;

namespace Atlas.Lite.Web.Tests;

public sealed class ExceptionStatusCodeMapperTests
{
    [Fact]
    public void GetStatusCode_MapsKnownExceptions()
    {
        Assert.Equal(
            StatusCodes.Status400BadRequest,
            ExceptionStatusCodeMapper.GetStatusCode(new ValidationException(new Dictionary<string, string[]>())));
        Assert.Equal(
            StatusCodes.Status404NotFound,
            ExceptionStatusCodeMapper.GetStatusCode(new NotFoundException("Todo", Guid.NewGuid())));
        Assert.Equal(
            StatusCodes.Status409Conflict,
            ExceptionStatusCodeMapper.GetStatusCode(new ConflictException("Duplicate item.")));
        Assert.Equal(
            StatusCodes.Status401Unauthorized,
            ExceptionStatusCodeMapper.GetStatusCode(new UnauthorizedAccessException()));
    }

    [Fact]
    public void GetStatusCode_MapsUnknownExceptionTo500()
    {
        Assert.Equal(
            StatusCodes.Status500InternalServerError,
            ExceptionStatusCodeMapper.GetStatusCode(new InvalidOperationException()));
    }
}
