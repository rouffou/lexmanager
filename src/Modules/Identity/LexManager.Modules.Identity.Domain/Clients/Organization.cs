using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Identity.Domain.Clients;

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
