using LexManager.Application.Abstractions.Pagination;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Documents.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.SearchDocuments;

public sealed class SearchDocumentsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/documents/search", async (
                string q,
                ISender sender,
                CancellationToken cancellationToken,
                Guid? caseId = null,
                int page = 1,
                int pageSize = 25) =>
            {
                Result<PagedList<DocumentSearchResultResponse>> result =
                    await sender.Send(new SearchDocumentsQuery(q, caseId, page, pageSize), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("SearchDocuments")
            .WithTags("Documents")
            .Produces<PagedList<DocumentSearchResultResponse>>()
            .ProducesValidationProblem();
    }
}
