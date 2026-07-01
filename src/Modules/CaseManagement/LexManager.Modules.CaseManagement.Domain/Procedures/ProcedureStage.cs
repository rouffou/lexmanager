namespace LexManager.Modules.CaseManagement.Domain.Procedures;

/// <summary>
/// One node of the procedure tree for a case. Owned by <see cref="ProcedurePlan"/>; its status moves
/// Pending → Current → Completed (or Skipped) as the case progresses, and it can carry a planned date
/// so the lawyer can situate it in the judicial calendar (SRD V11 §36).
/// </summary>
public sealed class ProcedureStage
{
    private ProcedureStage() { }

    internal ProcedureStage(int order, string name, string phase, ProcedureStageStatus status)
    {
        Order = order;
        Name = name;
        Phase = phase;
        Status = status;
    }

    public int Order { get; private set; }
    public string Name { get; private set; } = null!;
    public string Phase { get; private set; } = null!;
    public ProcedureStageStatus Status { get; private set; }
    public DateTime? PlannedOnUtc { get; private set; }
    public DateTime? CompletedOnUtc { get; private set; }

    internal void MarkCurrent() => Status = ProcedureStageStatus.Current;

    internal void Complete()
    {
        Status = ProcedureStageStatus.Completed;
        CompletedOnUtc = DateTime.UtcNow;
    }

    internal void Skip() => Status = ProcedureStageStatus.Skipped;

    internal void Schedule(DateTime plannedOnUtc) => PlannedOnUtc = plannedOnUtc;
}
