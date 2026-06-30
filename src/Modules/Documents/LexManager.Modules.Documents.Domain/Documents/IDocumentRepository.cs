namespace LexManager.Modules.Documents.Domain.Documents;

/// <summary>Write-side persistence port for the <see cref="Document"/> aggregate.</summary>
public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(DocumentId id, CancellationToken cancellationToken = default);

    void Add(Document document);
}
