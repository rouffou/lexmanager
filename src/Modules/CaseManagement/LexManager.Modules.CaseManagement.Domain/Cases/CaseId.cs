namespace LexManager.Modules.CaseManagement.Domain.Cases;

/// <summary>Strongly-typed identifier for a <see cref="Case"/>.</summary>
public readonly record struct CaseId(Guid Value)
{
    public static CaseId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
