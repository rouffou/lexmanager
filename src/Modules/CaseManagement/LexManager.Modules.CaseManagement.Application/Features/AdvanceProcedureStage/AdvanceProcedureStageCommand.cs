using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.AdvanceProcedureStage;

/// <summary>Passes the current stage of a procedure plan: completed (franchie) or, when
/// <paramref name="Skip"/> is set, marked not applicable (ignorée). Then promotes the next stage.</summary>
public sealed record AdvanceProcedureStageCommand(Guid ProcedurePlanId, bool Skip = false) : ICommand<Result>;
