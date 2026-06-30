namespace LexManager.Modules.Identity.Domain.Clients;

/// <summary>Whether a client is a natural person or a legal entity (SRD Module 1).</summary>
public enum ClientType
{
    PhysicalPerson = 1,
    LegalPerson = 2
}
