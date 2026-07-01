using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.ScheduleProcedureStage;

/// <summary>Sets the planned date of a procedure stage so it appears on the judicial calendar.</summary>
public sealed record ScheduleProcedureStageCommand(Guid ProcedurePlanId, int StageOrder, DateTime PlannedOnUtc)
    : ICommand<Result>;
