using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Domain.Cases;

public static class CaseErrors
{
    public static readonly ResultError NotFound = ResultError.NotFound(
        "Case.NotFound", "No case was found for the supplied identifier.");

    public static readonly ResultError ClientNotFound = ResultError.Problem(
        "Case.ClientNotFound", "The client to attach the case to does not exist.");

    public static readonly ResultError EmptyTitle = ResultError.Failure(
        "Case.EmptyTitle", "A case requires a title.");

    public static readonly ResultError AlreadyClosed = ResultError.Conflict(
        "Case.AlreadyClosed", "The case is already closed.");

    public static readonly ResultError CannotArchiveBeforeClosing = ResultError.Conflict(
        "Case.CannotArchiveBeforeClosing", "Only a closed case can be archived.");

    public static readonly ResultError InvalidJurisdiction = ResultError.Failure(
        "Case.InvalidJurisdiction", "The jurisdiction requires a court name and a general register (RG) number.");

    public static readonly ResultError InvalidAdverseParty = ResultError.Failure(
        "Case.InvalidAdverseParty", "An adverse party requires a name.");
}
