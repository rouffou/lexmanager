using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.GetClients;

/// <summary>Paginated client list (API is paginated end-to-end, SRD §5.2).</summary>
public sealed record GetClientsQuery(int Page = 1, int PageSize = 25, string? Search = null)
    : IQuery<Result<PagedList<ClientSummaryResponse>>>;
