using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Domain.Common;

public static class BillingErrors
{
    public static readonly ResultError NotFound = ResultError.NotFound(
        "Billing.NotFound", "No billing document was found for the supplied identifier.");

    public static readonly ResultError CaseNotFound = ResultError.Problem(
        "Billing.CaseNotFound", "The referenced case does not exist.");

    public static readonly ResultError ClientNotFound = ResultError.Problem(
        "Billing.ClientNotFound", "The referenced client does not exist.");

    public static readonly ResultError NegativeAmount = ResultError.Failure(
        "Billing.NegativeAmount", "A monetary amount cannot be negative.");

    public static readonly ResultError CurrencyMismatch = ResultError.Failure(
        "Billing.CurrencyMismatch", "Amounts in different currencies cannot be combined.");

    public static readonly ResultError NotEditable = ResultError.Conflict(
        "Billing.NotEditable", "Only a draft document can be modified.");

    public static readonly ResultError CannotIssueEmpty = ResultError.Conflict(
        "Billing.CannotIssueEmpty", "A document must have at least one line before it can be issued.");

    public static readonly ResultError NotIssued = ResultError.Conflict(
        "Billing.NotIssued", "Only an issued document can receive a payment or fall overdue.");

    public static readonly ResultError NoBillableTime = ResultError.Conflict(
        "Billing.NoBillableTime", "There is no billable time recorded for this case.");
}
