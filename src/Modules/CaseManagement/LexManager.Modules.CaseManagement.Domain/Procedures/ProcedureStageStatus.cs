namespace LexManager.Modules.CaseManagement.Domain.Procedures;

/// <summary>Where a single stage of the procedure tree stands.</summary>
public enum ProcedureStageStatus
{
    /// <summary>Not yet reached.</summary>
    Pending = 1,

    /// <summary>The stage the case is currently at — highlighted in the tree.</summary>
    Current = 2,

    /// <summary>Passed (franchie).</summary>
    Completed = 3,

    /// <summary>Not applicable to this case (ignorée).</summary>
    Skipped = 4,
}
