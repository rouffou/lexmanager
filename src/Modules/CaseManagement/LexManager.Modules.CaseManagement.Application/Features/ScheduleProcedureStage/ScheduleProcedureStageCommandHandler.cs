using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Domain.Procedures;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.ScheduleProcedureStage;

public sealed class ScheduleProcedureStageCommandHandler(
    IProcedurePlanRepository procedureRepository,
    ICaseUnitOfWork unitOfWork) : ICommandHandler<ScheduleProcedureStageCommand, Result>
{
    public async Task<Result> Handle(ScheduleProcedureStageCommand request, CancellationToken cancellationToken = default)
    {
        ProcedurePlan? plan =
            await procedureRepository.GetByIdAsync(new ProcedurePlanId(request.ProcedurePlanId), cancellationToken);
        if (plan is null)
        {
            return Result.Failure(ProcedureErrors.NotFound);
        }

        if (plan.Stages.All(stage => stage.Order != request.StageOrder))
        {
            return Result.Failure(ProcedureErrors.UnknownStage);
        }

        plan.ScheduleStage(request.StageOrder, request.PlannedOnUtc);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
