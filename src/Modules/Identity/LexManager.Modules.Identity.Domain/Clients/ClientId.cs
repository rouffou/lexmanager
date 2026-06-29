namespace LexManager.Modules.Identity.Domain.Clients;

/// <summary>Strongly-typed identifier for a <see cref="Client"/>.</summary>
public readonly record struct ClientId(Guid Value)
{
    public static ClientId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}

/// <summary>Whether a client is a natural person or a legal entity (SRD Module 1).</summary>
public enum ClientType
{
    PhysicalPerson = 1,
    LegalPerson = 2
}
