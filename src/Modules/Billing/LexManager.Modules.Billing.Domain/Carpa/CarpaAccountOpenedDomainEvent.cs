using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.Billing.Domain.Carpa;

public sealed record CarpaAccountOpenedDomainEvent(Guid AccountId, Guid CaseId, Guid ClientId) : IDomainEvent;
