using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.Calendar.Domain.Events;

public sealed record TimeEntryLoggedDomainEvent(Guid TimeEntryId, Guid CaseId, int DurationMinutes, bool IsBillable)
    : IDomainEvent;
