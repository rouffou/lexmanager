using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.Billing.Domain.Billing;

public sealed record BillingDocumentIssuedDomainEvent(Guid DocumentId, Guid CaseId, string Number, decimal Total)
    : IDomainEvent;

public sealed record PaymentRegisteredDomainEvent(Guid DocumentId, decimal Total, DateTime PaidOnUtc)
    : IDomainEvent;
