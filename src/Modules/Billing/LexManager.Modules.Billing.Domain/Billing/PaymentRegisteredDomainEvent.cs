using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.Billing.Domain.Billing;

public sealed record PaymentRegisteredDomainEvent(Guid DocumentId, decimal Total, DateTime PaidOnUtc)
    : IDomainEvent;
