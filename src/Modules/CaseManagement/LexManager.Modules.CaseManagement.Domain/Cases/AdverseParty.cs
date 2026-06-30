using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.CaseManagement.Domain.Cases;

/// <summary>An opposing party and (optionally) its counsel — the adverse lawyer.</summary>
public sealed class AdverseParty : ValueObject
{
    private AdverseParty(string name, string? counsel)
    {
        Name = name;
        Counsel = counsel;
    }

    public string Name { get; }
    public string? Counsel { get; }

    public static AdverseParty Create(string name, string? counsel = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException(CaseErrors.InvalidAdverseParty);
        }

        return new AdverseParty(name.Trim(), string.IsNullOrWhiteSpace(counsel) ? null : counsel.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Counsel;
    }
}
