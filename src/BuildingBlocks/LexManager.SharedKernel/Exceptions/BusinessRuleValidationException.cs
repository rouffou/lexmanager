using Mediarq.Core.Common.Results;

namespace LexManager.SharedKernel.Exceptions;

/// <summary>
/// Raised when a semantic (domain-level) business rule is violated, e.g. closing a
/// case that still has unpaid invoices. Prefer the <c>Result</c> pattern for flows
/// where the caller is expected to branch on the outcome; reserve this for invariants
/// that must never be reachable through valid use of the public domain API.
/// </summary>
public sealed class BusinessRuleValidationException(ResultError error) : DomainException(error);
