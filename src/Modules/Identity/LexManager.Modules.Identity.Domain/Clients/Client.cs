using LexManager.Modules.Identity.Domain.Clients.Events;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Identity.Domain.Clients;

/// <summary>
/// A client of the firm — a natural person or a legal entity. Aggregate root: state changes
/// go through explicit behaviour (no public setters), enforcing the domain's invariants
/// (semantic validation, Normes §3.2).
/// </summary>
public sealed class Client : AggregateRoot<ClientId>
{
    // EF Core materialization.
    private Client() { }

    private Client(ClientId id, ClientType type, Email email, string? phone) : base(id)
    {
        Type = type;
        Email = email;
        Phone = phone;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public ClientType Type { get; private set; }
    public Email Email { get; private set; } = null!;
    public string? Phone { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    // Physical person.
    public PersonName? Name { get; private set; }
    public string? NationalIdentityNumber { get; private set; }

    // Legal entity.
    public Organization? Organization { get; private set; }

    /// <summary>The conflict-of-interest key used for the mandatory global search at creation time.</summary>
    public string IdentityKey =>
        Type == ClientType.PhysicalPerson ? NationalIdentityNumber! : Organization!.RegistrationNumber;

    public string DisplayName =>
        Type == ClientType.PhysicalPerson ? Name!.FullName : Organization!.CompanyName;

    public static Client CreatePhysicalPerson(
        PersonName name,
        string nationalIdentityNumber,
        Email email,
        string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(nationalIdentityNumber))
        {
            throw new BusinessRuleValidationException(ClientErrors.MissingIdentityNumber);
        }

        var client = new Client(ClientId.New(), ClientType.PhysicalPerson, email, phone)
        {
            Name = name,
            NationalIdentityNumber = nationalIdentityNumber.Trim()
        };

        client.Raise(new ClientCreatedDomainEvent(client.Id.Value, client.Type, client.DisplayName));
        return client;
    }

    public static Client CreateLegalPerson(Organization organization, Email email, string? phone = null)
    {
        var client = new Client(ClientId.New(), ClientType.LegalPerson, email, phone)
        {
            Organization = organization
        };

        client.Raise(new ClientCreatedDomainEvent(client.Id.Value, client.Type, client.DisplayName));
        return client;
    }

    public void UpdateContactInfo(Email email, string? phone)
    {
        Email = email;
        Phone = phone;
    }
}
