using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.GetClients;

/// <summary>Paginated client list (API is paginated end-to-end, SRD §5.2).</summary>
public sealed record GetClientsQuery(int Page = 1, int PageSize = 25, string? Search = null)
    : IQuery<Result<PagedList<ClientSummaryResponse>>>;

public sealed class GetClientsQueryHandler(IClientReadRepository readRepository)
    : IQueryHandler<GetClientsQuery, Result<PagedList<ClientSummaryResponse>>>
{
    public async Task<Result<PagedList<ClientSummaryResponse>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new PaginationParameters(request.Page, request.PageSize);
        PagedList<ClientSummaryResponse> page = await readRepository.GetPagedAsync(parameters, request.Search, cancellationToken);

        return Result.Success(page);
    }
}
