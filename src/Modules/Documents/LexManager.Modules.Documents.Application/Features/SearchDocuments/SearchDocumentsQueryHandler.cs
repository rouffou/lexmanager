using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.SearchDocuments;

public sealed class SearchDocumentsQueryHandler(IDocumentReadRepository readRepository)
    : IQueryHandler<SearchDocumentsQuery, Result<PagedList<DocumentSearchResultResponse>>>
{
    public async Task<Result<PagedList<DocumentSearchResultResponse>>> Handle(
        SearchDocumentsQuery request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new PaginationParameters(request.Page, request.PageSize);
        PagedList<DocumentSearchResultResponse> page =
            await readRepository.SearchAsync(request.Term.Trim(), request.CaseId, parameters, cancellationToken);

        return Result.Success(page);
    }
}
