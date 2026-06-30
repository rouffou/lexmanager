using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.CaseManagement.Domain.Cases;

/// <summary>Court information for the instance (SRD Module 2: juridiction, n° RG, juge).</summary>
public sealed class Jurisdiction : ValueObject
{
    private Jurisdiction(string courtName, string generalRegisterNumber, string? judge)
    {
        CourtName = courtName;
        GeneralRegisterNumber = generalRegisterNumber;
        Judge = judge;
    }

    public string CourtName { get; }

    /// <summary>Numéro de RG (Répertoire Général).</summary>
    public string GeneralRegisterNumber { get; }

    public string? Judge { get; }

    public static Jurisdiction Create(string courtName, string generalRegisterNumber, string? judge = null)
    {
        if (string.IsNullOrWhiteSpace(courtName) || string.IsNullOrWhiteSpace(generalRegisterNumber))
        {
            throw new BusinessRuleValidationException(CaseErrors.InvalidJurisdiction);
        }

        return new Jurisdiction(courtName.Trim(), generalRegisterNumber.Trim(), string.IsNullOrWhiteSpace(judge) ? null : judge.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CourtName;
        yield return GeneralRegisterNumber;
        yield return Judge;
    }
}
