namespace LexManager.Modules.CaseManagement.Contracts;

public sealed record ProcedureStageResponse(
    int Order,
    string Name,
    string Phase,
    string Status,
    DateTime? PlannedOnUtc,
    DateTime? CompletedOnUtc);
