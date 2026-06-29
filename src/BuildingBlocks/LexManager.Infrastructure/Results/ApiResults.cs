using Mediarq.Core.Common.Results;
using Microsoft.AspNetCore.Http;

namespace LexManager.Infrastructure.Results;

/// <summary>
/// Bridges Mediarq's <see cref="Result"/> pattern to Minimal API <see cref="IResult"/> responses.
/// Failures become RFC 7807 ProblemDetails; the HTTP status is derived from <see cref="ErrorType"/>.
/// </summary>
public static class ApiResults
{
    public static IResult ToApiResult(this Result result, Func<IResult>? onSuccess = null) =>
        result.IsSuccess
            ? onSuccess?.Invoke() ?? Microsoft.AspNetCore.Http.Results.NoContent()
            : Problem(result.Error);

    public static IResult ToApiResult<TValue>(this Result<TValue> result, Func<TValue, IResult>? onSuccess = null) =>
        result.IsSuccess
            ? onSuccess?.Invoke(result.Value) ?? Microsoft.AspNetCore.Http.Results.Ok(result.Value)
            : Problem(result.Error);

    public static int ToStatusCode(this ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Problem => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
    };

    public static IResult Problem(ResultError error)
    {
        if (error is ValidationError validation)
        {
            Dictionary<string, string[]> errors = validation.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());

            return Microsoft.AspNetCore.Http.Results.ValidationProblem(
                errors,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Microsoft.AspNetCore.Http.Results.Problem(
            title: error.Code,
            detail: error.Message,
            statusCode: error.Type.ToStatusCode());
    }
}
