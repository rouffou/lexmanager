namespace LexManager.Modules.CaseManagement.Contracts;

public sealed record ProcedurePlanResponse(
    Guid Id,
    Guid CaseId,
    string Type,
    DateTime ReferenceOnUtc,
    int ProgressPercent,
    int? CurrentStageOrder,
    bool IsComplete,
    IReadOnlyList<ProcedureStageResponse> Stages,
    DateTime CreatedOnUtc);
