using LexManager.Modules.Documents.Domain.Documents.Events;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Documents.Domain.Documents;

/// <summary>
/// A document attached to a case (procedure piece, conclusions, contract, generated letter…).
/// Aggregate root owning an append-only history of <see cref="DocumentVersion"/> (SRD Module 3).
/// Linked to a case by id only — the case lives in the Case Management module (SRD §3.2).
/// </summary>
public sealed class Document : AggregateRoot<DocumentId>
{
    private readonly List<DocumentVersion> _versions = [];

    private Document() { }

    private Document(DocumentId id, Guid caseId, string fileName, string contentType, DocumentCategory category, bool isConfidential)
        : base(id)
    {
        CaseId = caseId;
        FileName = fileName;
        ContentType = contentType;
        Category = category;
        IsConfidential = isConfidential;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid CaseId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public DocumentCategory Category { get; private set; }

    /// <summary>Confidential pieces are visible only to the lawyer in charge (SRD §5.1 RBAC).</summary>
    public bool IsConfidential { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public IReadOnlyList<DocumentVersion> Versions => _versions.AsReadOnly();
    public int CurrentVersionNumber => _versions.Count == 0 ? 0 : _versions.Max(version => version.VersionNumber);

    public static Document Create(
        Guid caseId,
        string fileName,
        string contentType,
        DocumentCategory category,
        string storageKey,
        long sizeBytes,
        string checksum,
        bool isConfidential = false)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new BusinessRuleValidationException(DocumentErrors.MissingFileName);
        }

        var document = new Document(DocumentId.New(), caseId, fileName.Trim(), contentType, category, isConfidential);
        document._versions.Add(new DocumentVersion(1, storageKey, sizeBytes, checksum));
        document.Raise(new DocumentUploadedDomainEvent(document.Id.Value, caseId, document.FileName));
        return document;
    }

    public DocumentVersion AddVersion(string storageKey, long sizeBytes, string checksum)
    {
        var version = new DocumentVersion(CurrentVersionNumber + 1, storageKey, sizeBytes, checksum);
        _versions.Add(version);
        Raise(new DocumentVersionAddedDomainEvent(Id.Value, version.VersionNumber));
        return version;
    }

    public DocumentVersion GetVersion(int versionNumber) =>
        _versions.SingleOrDefault(version => version.VersionNumber == versionNumber)
            ?? throw new BusinessRuleValidationException(DocumentErrors.NotFound);

    public DocumentVersion CurrentVersion => GetVersion(CurrentVersionNumber);
}
