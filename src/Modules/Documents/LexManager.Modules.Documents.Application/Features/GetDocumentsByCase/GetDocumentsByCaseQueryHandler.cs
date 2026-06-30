using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.GetDocumentsByCase;

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
