using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.CaseManagement.Domain.Cases.Events;

public sealed record CaseOpenedDomainEvent(Guid CaseId, Guid ClientId, string Title) : IDomainEvent;

public sealed record CaseClosedDomainEvent(Guid CaseId, DateTime ClosedOnUtc) : IDomainEvent;
