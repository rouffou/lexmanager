using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Documents.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.GetDocumentsByCase;

public sealed record GetDocumentsByCaseQuery(Guid CaseId, int Page = 1, int PageSize = 25)
    : IQuery<Result<PagedList<DocumentSummaryResponse>>>;
