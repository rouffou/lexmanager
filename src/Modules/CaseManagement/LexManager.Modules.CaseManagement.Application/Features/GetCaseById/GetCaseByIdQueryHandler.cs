using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.CaseManagement.Domain.Cases;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.GetCaseById;

public sealed class GetCaseByIdQueryHandler(ICaseReadRepository readRepository)
    : IQueryHandler<GetCaseByIdQuery, Result<CaseResponse>>
{
    public async Task<Result<CaseResponse>> Handle(GetCaseByIdQuery request, CancellationToken cancellationToken = default)
    {
        CaseResponse? @case = await readRepository.GetByIdAsync(request.CaseId, cancellationToken);

        return @case is null
            ? Result.Failure<CaseResponse>(CaseErrors.NotFound)
            : Result.Success(@case);
    }
}
