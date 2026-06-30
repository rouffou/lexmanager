namespace LexManager.SharedKernel.Domain;

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
