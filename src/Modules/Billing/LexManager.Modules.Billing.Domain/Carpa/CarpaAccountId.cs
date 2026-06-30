namespace LexManager.Modules.Billing.Domain.Carpa;

/// <summary>Strongly-typed identifier for a <see cref="CarpaAccount"/>.</summary>
public readonly record struct CarpaAccountId(Guid Value)
{
    public static CarpaAccountId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
