using Atlas.Lite.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Atlas.Lite.Web.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = ExceptionStatusCodeMapper.GetStatusCode(exception);
        httpContext.Response.StatusCode = statusCode;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}.", httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "Handled exception while processing {Method} {Path}.", httpContext.Request.Method, httpContext.Request.Path);
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(exception, statusCode),
            Detail = environment.IsDevelopment() ? exception.Message : GetPublicDetail(statusCode),
            Instance = httpContext.Request.Path
        };

        if (exception is AtlasLiteException atlasLiteException)
        {
            problem.Extensions["code"] = atlasLiteException.Code;
        }

        if (exception is ValidationException validationException)
        {
            problem.Extensions["errors"] = validationException.Errors;
        }

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problem
        });
    }

    private static string GetTitle(Exception exception, int statusCode)
    {
        return exception switch
        {
            ValidationException => "Validation failed",
            NotFoundException => "Resource not found",
            ConflictException => "Conflict",
            UnauthorizedAccessException => "Unauthorized",
            _ when statusCode >= StatusCodes.Status500InternalServerError => "Server error",
            _ => "Request failed"
        };
    }

    private static string GetPublicDetail(int statusCode)
    {
        return statusCode >= StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred."
            : "The request could not be processed.";
    }
}
