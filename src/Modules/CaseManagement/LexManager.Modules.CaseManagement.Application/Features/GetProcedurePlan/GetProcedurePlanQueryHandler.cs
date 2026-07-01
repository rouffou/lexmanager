using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.CaseManagement.Domain.Procedures;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.GetProcedurePlan;

public sealed class GetProcedurePlanQueryHandler(IProcedureReadRepository readRepository)
    : IQueryHandler<GetProcedurePlanQuery, Result<ProcedurePlanResponse>>
{
    public async Task<Result<ProcedurePlanResponse>> Handle(
        GetProcedurePlanQuery request,
        CancellationToken cancellationToken = default)
    {
        ProcedurePlanResponse? plan = await readRepository.GetByCaseAsync(request.CaseId, cancellationToken);

        return plan is null
            ? Result.Failure<ProcedurePlanResponse>(ProcedureErrors.NotFound)
            : Result.Success(plan);
    }
}
