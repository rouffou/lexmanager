using LexManager.Modules.Documents.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Documents.Infrastructure.Persistence;

internal sealed class DocumentRepository(DocumentsDbContext context) : IDocumentRepository
{
    public Task<Document?> GetByIdAsync(DocumentId id, CancellationToken cancellationToken = default) =>
        context.Documents.FirstOrDefaultAsync(document => document.Id == id, cancellationToken);

    public void Add(Document document) => context.Documents.Add(document);
}
