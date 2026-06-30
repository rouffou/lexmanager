using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.Billing.Domain.Carpa;

public sealed record CarpaMovementRecordedDomainEvent(Guid AccountId, CarpaTransactionType Type, decimal Amount, decimal NewBalance)
    : IDomainEvent;
