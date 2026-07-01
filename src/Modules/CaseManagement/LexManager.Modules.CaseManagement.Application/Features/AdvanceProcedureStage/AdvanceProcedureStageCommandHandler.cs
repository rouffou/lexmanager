using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Domain.Procedures;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.AdvanceProcedureStage;

public sealed class AdvanceProcedureStageCommandHandler(
    IProcedurePlanRepository procedureRepository,
    ICaseUnitOfWork unitOfWork) : ICommandHandler<AdvanceProcedureStageCommand, Result>
{
    public async Task<Result> Handle(AdvanceProcedureStageCommand request, CancellationToken cancellationToken = default)
    {
        ProcedurePlan? plan =
            await procedureRepository.GetByIdAsync(new ProcedurePlanId(request.ProcedurePlanId), cancellationToken);
        if (plan is null)
        {
            return Result.Failure(ProcedureErrors.NotFound);
        }

        if (plan.CurrentStage is null)
        {
            return Result.Failure(ProcedureErrors.AlreadyComplete);
        }

        if (request.Skip)
        {
            plan.SkipCurrentStage();
        }
        else
        {
            plan.AdvanceCurrentStage();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
