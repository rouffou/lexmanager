namespace LexManager.SharedKernel.Domain;

/// <summary>
/// Marker for aggregate roots — the only entities a repository may load/persist directly.
/// </summary>
public interface IAggregateRoot;
