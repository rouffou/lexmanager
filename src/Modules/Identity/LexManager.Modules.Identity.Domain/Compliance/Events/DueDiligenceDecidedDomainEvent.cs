using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.Identity.Domain.Compliance.Events;

public sealed record DueDiligenceDecidedDomainEvent(
    Guid DueDiligenceId,
    Guid ClientId,
    DueDiligenceStatus Status,
    int ComplianceScore) : IDomainEvent;
