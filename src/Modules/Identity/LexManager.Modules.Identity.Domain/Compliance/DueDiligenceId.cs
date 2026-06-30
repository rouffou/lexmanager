namespace LexManager.Modules.Identity.Domain.Compliance;

/// <summary>Strongly-typed identifier for a <see cref="ClientDueDiligence"/> file.</summary>
public readonly record struct DueDiligenceId(Guid Value)
{
    public static DueDiligenceId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
