using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Domain.Cases;
using LexManager.Modules.CaseManagement.Domain.Procedures;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.GenerateProcedurePlan;

public sealed class GenerateProcedurePlanCommandHandler(
    ICaseRepository caseRepository,
    IProcedurePlanRepository procedureRepository,
    ICaseUnitOfWork unitOfWork) : ICommandHandler<GenerateProcedurePlanCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(GenerateProcedurePlanCommand request, CancellationToken cancellationToken = default)
    {
        Case? @case = await caseRepository.GetByIdAsync(new CaseId(request.CaseId), cancellationToken);
        if (@case is null)
        {
            return Result.Failure<Guid>(ProcedureErrors.CaseNotFound);
        }

        if (await procedureRepository.ExistsForCaseAsync(request.CaseId, cancellationToken))
        {
            return Result.Failure<Guid>(ProcedureErrors.AlreadyExistsForCase);
        }

        ProcedurePlan plan = ProcedurePlan.Generate(request.CaseId, request.ProcedureType, request.ReferenceOnUtc);

        procedureRepository.Add(plan);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(plan.Id.Value);
    }
}
