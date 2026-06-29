namespace LexManager.SharedKernel.Domain;

/// <summary>
/// Base class for value objects: equality is based on the structural components,
/// not identity. Value objects are immutable.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>The atomic values that make up this value object's identity.</summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other) =>
        other is not null && GetType() == other.GetType() &&
        GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override bool Equals(object? obj) => obj is ValueObject vo && Equals(vo);

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Aggregate(new HashCode(), (hash, component) =>
            {
                hash.Add(component);
                return hash;
            })
            .ToHashCode();

    public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);

    public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);
}
