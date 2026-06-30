namespace LexManager.SharedKernel.Domain;

/// <summary>
/// Marker for aggregate roots — the only entities a repository may load/persist directly.
/// </summary>
public interface IAggregateRoot;

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

/// <summary>
/// Base class for aggregate roots. Collects domain events raised during a unit of work,
/// to be dispatched (via Mediarq) after the transaction commits.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot, IHasDomainEvents
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(TId id) : base(id) { }

    protected AggregateRoot() { }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
