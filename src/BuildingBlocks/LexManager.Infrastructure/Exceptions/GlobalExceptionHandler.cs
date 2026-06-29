using LexManager.SharedKernel.Exceptions;
using LexManager.Infrastructure.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LexManager.Infrastructure.Exceptions;

/// <summary>
/// Converts unhandled exceptions into standardized RFC 7807 ProblemDetails responses
/// (Normes §1.2). Expected domain-rule violations carry a structured error; everything
/// else is logged and surfaced as a generic 500 without leaking internals.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case DomainException domainException:
                logger.LogWarning(domainException,
                    "Domain rule violation: {Code}", domainException.Error.Code);

                await ApiResults.Problem(domainException.Error).ExecuteAsync(httpContext);
                return true;

            default:
                logger.LogError(exception, "Unhandled exception processing {Path}", httpContext.Request.Path);

                await Microsoft.AspNetCore.Http.Results.Problem(
                        title: "An unexpected error occurred.",
                        statusCode: StatusCodes.Status500InternalServerError)
                    .ExecuteAsync(httpContext);
                return true;
        }
    }
}
