using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.Identity.Domain.Compliance.Events;

public sealed record DueDiligenceStartedDomainEvent(Guid DueDiligenceId, Guid ClientId, RiskLevel RiskLevel) : IDomainEvent;
