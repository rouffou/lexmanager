namespace LexManager.Modules.CaseManagement.Domain.Procedures;

/// <summary>Strongly-typed identifier for a <see cref="ProcedurePlan"/>.</summary>
public readonly record struct ProcedurePlanId(Guid Value)
{
    public static ProcedurePlanId New() => new(Guid.NewGuid());
}
