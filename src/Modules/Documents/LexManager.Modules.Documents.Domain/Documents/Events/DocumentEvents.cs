using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.Documents.Domain.Documents.Events;

public sealed record DocumentUploadedDomainEvent(Guid DocumentId, Guid CaseId, string FileName) : IDomainEvent;

public sealed record DocumentVersionAddedDomainEvent(Guid DocumentId, int VersionNumber) : IDomainEvent;
