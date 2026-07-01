using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Documents.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.SearchDocuments;

/// <summary>Full-text search across the DMS, optionally narrowed to a single case (SRD §7.2).</summary>
public sealed record SearchDocumentsQuery(string Term, Guid? CaseId = null, int Page = 1, int PageSize = 25)
    : IQuery<Result<PagedList<DocumentSearchResultResponse>>>;
