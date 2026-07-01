using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Domain.Procedures;

public static class ProcedureErrors
{
    public static readonly ResultError NotFound = ResultError.NotFound(
        "Procedure.NotFound", "No procedure plan was found for the supplied identifier.");

    public static readonly ResultError CaseNotFound = ResultError.Problem(
        "Procedure.CaseNotFound", "The case to attach the procedure plan to does not exist.");

    public static readonly ResultError AlreadyExistsForCase = ResultError.Conflict(
        "Procedure.AlreadyExistsForCase", "This case already has a procedure plan.");

    public static readonly ResultError AlreadyComplete = ResultError.Conflict(
        "Procedure.AlreadyComplete", "Every stage of the procedure has already been passed.");

    public static readonly ResultError UnknownStage = ResultError.NotFound(
        "Procedure.UnknownStage", "No stage with the supplied order exists in this procedure plan.");

    public static readonly ResultError EmptyProcedure = ResultError.Failure(
        "Procedure.EmptyProcedure", "The selected procedure type has no defined stages.");
}
