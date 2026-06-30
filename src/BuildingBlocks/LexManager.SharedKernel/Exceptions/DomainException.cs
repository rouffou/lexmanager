using Mediarq.Core.Common.Results;

namespace LexManager.SharedKernel.Exceptions;

/// <summary>
/// Base class for expected domain-rule violations. Carries a structured
/// <see cref="ResultError"/> so the global exception middleware can translate
/// it into an RFC 7807 ProblemDetails response without leaking internals.
/// </summary>
public abstract class DomainException(ResultError error) : Exception(error.Message)
{
    public ResultError Error { get; } = error;
}
