using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Contracts;
using LexManager.Modules.Identity.Domain.Clients;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.GetClientById;

public sealed record GetClientByIdQuery(Guid ClientId) : IQuery<Result<ClientResponse>>;

public sealed class GetClientByIdQueryHandler(IClientReadRepository readRepository)
    : IQueryHandler<GetClientByIdQuery, Result<ClientResponse>>
{
    public async Task<Result<ClientResponse>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken = default)
    {
        ClientResponse? client = await readRepository.GetByIdAsync(request.ClientId, cancellationToken);

        return client is null
            ? Result.Failure<ClientResponse>(ClientErrors.NotFound)
            : Result.Success(client);
    }
}
