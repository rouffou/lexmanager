using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.Calendar.Domain.Events;

public sealed record CalendarEventScheduledDomainEvent(Guid EventId, Guid OwnerUserId, DateTime StartUtc, DateTime EndUtc)
    : IDomainEvent;
