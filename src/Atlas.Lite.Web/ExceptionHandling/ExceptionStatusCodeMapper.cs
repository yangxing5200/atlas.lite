using Atlas.Lite.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Atlas.Lite.Web.ExceptionHandling;

public static class ExceptionStatusCodeMapper
{
    public static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
