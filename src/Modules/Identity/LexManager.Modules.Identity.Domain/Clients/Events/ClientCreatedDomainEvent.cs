using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.Identity.Domain.Clients.Events;

/// <summary>
/// Raised when a new client is registered. Other modules (e.g. Case Management) can react
/// in-process via an integration event handler — never by reaching into Identity's database.
/// </summary>
public sealed record ClientCreatedDomainEvent(Guid ClientId, ClientType Type, string DisplayName) : IDomainEvent;
