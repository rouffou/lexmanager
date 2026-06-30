namespace LexManager.SharedKernel.Domain;

/// <summary>
/// Non-generic view over an aggregate's pending domain events, so a DbContext can collect and
/// dispatch them without knowing the aggregate's strongly-typed id (useful when one context holds
/// several aggregate types).
/// </summary>
public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}
