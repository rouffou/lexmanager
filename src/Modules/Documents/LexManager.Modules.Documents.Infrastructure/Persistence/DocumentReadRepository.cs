using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Contracts;
using LexManager.Modules.Documents.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Documents.Infrastructure.Persistence;

internal sealed class DocumentReadRepository(DocumentsDbContext context) : IDocumentReadRepository
{
    public async Task<DocumentResponse?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var id = new DocumentId(documentId);

        Document? document = await context.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        return document is null ? null : MapFull(document);
    }

    public async Task<PagedList<DocumentSummaryResponse>> GetByCaseAsync(
        Guid caseId,
        PaginationParameters parameters,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Document> query = context.Documents.AsNoTracking().Where(d => d.CaseId == caseId);

        int totalCount = await query.CountAsync(cancellationToken);

        List<Document> documents = await query
            .OrderByDescending(d => d.CreatedOnUtc)
            .Skip(parameters.Skip)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        IReadOnlyList<DocumentSummaryResponse> items = documents.Select(MapSummary).ToList();

        return new PagedList<DocumentSummaryResponse>(items, parameters.Page, parameters.PageSize, totalCount);
    }

    public async Task<DocumentFileRef?> GetFileRefAsync(Guid documentId, int? versionNumber, CancellationToken cancellationToken = default)
    {
        var id = new DocumentId(documentId);

        Document? document = await context.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (document is null)
        {
            return null;
        }

        DocumentVersion? version = versionNumber is null
            ? document.Versions.SingleOrDefault(v => v.VersionNumber == document.CurrentVersionNumber)
            : document.Versions.SingleOrDefault(v => v.VersionNumber == versionNumber.Value);

        return version is null ? null : new DocumentFileRef(document.FileName, document.ContentType, version.StorageKey);
    }

    public Task<int> CountByCaseAsync(Guid caseId, CancellationToken cancellationToken = default) =>
        context.Documents.AsNoTracking().CountAsync(d => d.CaseId == caseId, cancellationToken);

    private static DocumentResponse MapFull(Document document) => new(
        document.Id.Value,
        document.CaseId,
        document.FileName,
        document.ContentType,
        document.Category.ToString(),
        document.IsConfidential,
        document.CurrentVersionNumber,
        document.Versions
            .OrderBy(v => v.VersionNumber)
            .Select(v => new DocumentVersionResponse(v.VersionNumber, v.SizeBytes, v.Checksum, v.UploadedOnUtc))
            .ToList(),
        document.CreatedOnUtc);

    private static DocumentSummaryResponse MapSummary(Document document) => new(
        document.Id.Value,
        document.CaseId,
        document.FileName,
        document.Category.ToString(),
        document.CurrentVersionNumber,
        document.CreatedOnUtc);
}
