using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Domain.Carpa;

public static class CarpaErrors
{
    public static readonly ResultError NotFound = ResultError.NotFound(
        "Carpa.NotFound", "No third-party account was found for the supplied identifier.");

    public static readonly ResultError CaseNotFound = ResultError.Problem(
        "Carpa.CaseNotFound", "The referenced case does not exist.");

    public static readonly ResultError ClientNotFound = ResultError.Problem(
        "Carpa.ClientNotFound", "The referenced client does not exist.");

    public static readonly ResultError NonPositiveAmount = ResultError.Failure(
        "Carpa.NonPositiveAmount", "A movement amount must be strictly positive.");

    public static readonly ResultError InsufficientFunds = ResultError.Conflict(
        "Carpa.InsufficientFunds", "Third-party funds cannot be overdrawn: the disbursement exceeds the available balance.");

    public static readonly ResultError AlreadyExistsForCase = ResultError.Conflict(
        "Carpa.AlreadyExistsForCase", "A third-party account already exists for this case.");
}
