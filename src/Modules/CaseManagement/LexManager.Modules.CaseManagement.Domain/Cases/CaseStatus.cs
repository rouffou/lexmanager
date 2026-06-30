namespace LexManager.Modules.CaseManagement.Domain.Cases;

/// <summary>Lifecycle of a case file (SRD Module 2: ouverture, instruction, archivage, clôture).</summary>
public enum CaseStatus
{
    Opened = 1,
    UnderInvestigation = 2,
    Closed = 3
}
