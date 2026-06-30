namespace LexManager.Modules.Identity.Domain.Compliance;

/// <summary>Lifecycle of a client due-diligence file (LCB-FT, SRD V11 §30).</summary>
public enum DueDiligenceStatus
{
    InProgress = 1,
    Approved = 2,
    Rejected = 3
}
