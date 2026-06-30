using LexManager.Application.Abstractions.Pagination;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.GetDocumentsByCase;

public sealed record GetDocumentsByCaseQuery(Guid CaseId, int Page = 1, int PageSize = 25)
    : IQuery<Result<PagedList<DocumentSummaryResponse>>>;

public sealed class GetDocumentsByCaseQueryHandler(IDocumentReadRepository readRepository)
    : IQueryHandler<GetDocumentsByCaseQuery, Result<PagedList<DocumentSummaryResponse>>>
{
    public async Task<Result<PagedList<DocumentSummaryResponse>>> Handle(
        GetDocumentsByCaseQuery request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new PaginationParameters(request.Page, request.PageSize);
        PagedList<DocumentSummaryResponse> page =
            await readRepository.GetByCaseAsync(request.CaseId, parameters, cancellationToken);

        return Result.Success(page);
    }
}

public sealed class GetDocumentsByCaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/documents", async (
                Guid caseId,
                ISender sender,
                CancellationToken cancellationToken,
                int page = 1,
                int pageSize = 25) =>
            {
                Result<PagedList<DocumentSummaryResponse>> result =
                    await sender.Send(new GetDocumentsByCaseQuery(caseId, page, pageSize), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetDocumentsByCase")
            .WithTags("Documents")
            .Produces<PagedList<DocumentSummaryResponse>>();
    }
}
