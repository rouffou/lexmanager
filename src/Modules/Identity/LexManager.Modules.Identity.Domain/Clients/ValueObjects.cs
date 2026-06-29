using System.Text.RegularExpressions;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Domain.Clients;

/// <summary>A validated email address.</summary>
public sealed partial class Email : ValueObject
{
    private Email(string value) => Value = value;

    public string Value { get; }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !EmailRegex().IsMatch(value))
        {
            throw new BusinessRuleValidationException(ClientErrors.InvalidEmail);
        }

        return new Email(value.Trim().ToLowerInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegex();
}

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

/// <summary>A legal entity's identifying details (SRD Module 1: raison sociale, SIRET, représentant légal).</summary>
public sealed class Organization : ValueObject
{
    private Organization(string companyName, string registrationNumber, string legalRepresentative)
    {
        CompanyName = companyName;
        RegistrationNumber = registrationNumber;
        LegalRepresentative = legalRepresentative;
    }

    public string CompanyName { get; }

    /// <summary>French SIRET — exactly 14 digits (Normes §3.1).</summary>
    public string RegistrationNumber { get; }

    public string LegalRepresentative { get; }

    public static Organization Create(string companyName, string registrationNumber, string legalRepresentative)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            throw new BusinessRuleValidationException(ClientErrors.MissingCompanyName);
        }

        string normalizedSiret = (registrationNumber ?? string.Empty).Replace(" ", string.Empty);
        if (normalizedSiret.Length != 14 || !normalizedSiret.All(char.IsDigit))
        {
            throw new BusinessRuleValidationException(ClientErrors.InvalidSiret);
        }

        return new Organization(companyName.Trim(), normalizedSiret, (legalRepresentative ?? string.Empty).Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CompanyName;
        yield return RegistrationNumber;
        yield return LegalRepresentative;
    }
}
