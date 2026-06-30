using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Contracts;
using LexManager.Modules.Identity.Domain.Compliance;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.GetClientDueDiligence;

public sealed class GetClientDueDiligenceQueryHandler(IDueDiligenceReadRepository readRepository)
    : IQueryHandler<GetClientDueDiligenceQuery, Result<DueDiligenceResponse>>
{
    public async Task<Result<DueDiligenceResponse>> Handle(GetClientDueDiligenceQuery request, CancellationToken cancellationToken = default)
    {
        DueDiligenceResponse? file = await readRepository.GetByClientAsync(request.ClientId, cancellationToken);
        return file is null
            ? Result.Failure<DueDiligenceResponse>(KycErrors.NotFound)
            : Result.Success(file);
    }
}
