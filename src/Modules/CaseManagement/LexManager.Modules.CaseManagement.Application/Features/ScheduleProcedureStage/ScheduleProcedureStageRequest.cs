namespace LexManager.Modules.CaseManagement.Application.Features.ScheduleProcedureStage;

/// <summary>Request body for scheduling a stage (plan id and stage order come from the route).</summary>
public sealed record ScheduleProcedureStageRequest(DateTime PlannedOnUtc);
