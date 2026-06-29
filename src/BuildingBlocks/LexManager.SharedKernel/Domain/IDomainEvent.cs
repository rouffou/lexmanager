using Mediarq.Core.Common.Requests.Notifications;

namespace LexManager.SharedKernel.Domain;

/// <summary>
/// Marker for in-process domain events. Dispatched via Mediarq's INotification
/// so handlers stay decoupled from the entity that raised the event.
/// </summary>
public interface IDomainEvent : INotification
{
    Guid EventId => Guid.NewGuid();
    DateTime OccurredOnUtc => DateTime.UtcNow;
}
