namespace LexManager.Modules.Identity.Domain.Clients;

/// <summary>Strongly-typed identifier for a <see cref="Client"/>.</summary>
public readonly record struct ClientId(Guid Value)
{
    public static ClientId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
