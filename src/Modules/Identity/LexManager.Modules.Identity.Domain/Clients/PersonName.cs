using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Identity.Domain.Clients;

/// <summary>The name of a natural person (SRD Module 1: nom, prénom).</summary>
public sealed class PersonName : ValueObject
{
    private PersonName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string FullName => $"{FirstName} {LastName}";

    public static PersonName Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            throw new BusinessRuleValidationException(ClientErrors.MissingPersonName);
        }

        return new PersonName(firstName.Trim(), lastName.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}
