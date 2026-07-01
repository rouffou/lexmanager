namespace LexManager.Modules.CaseManagement.Domain.Procedures;

/// <summary>
/// The kind of judicial procedure a case follows. Each value maps to an ordered milestone
/// blueprint in <see cref="ProcedureCatalog"/> (Belgian civil procedure, SRD V11 §36).
/// </summary>
public enum ProcedureType
{
    /// <summary>Recouvrement de créance — de la mise en demeure à l'exécution forcée.</summary>
    DebtRecovery = 1,

    /// <summary>Procédure civile au fond (tribunal de première instance).</summary>
    CivilLitigation = 2,

    /// <summary>Référé (procédure d'urgence).</summary>
    SummaryProceedings = 3,

    /// <summary>Litige social (tribunal du travail).</summary>
    LabourDispute = 4,

    /// <summary>Appel (cour d'appel / du travail).</summary>
    Appeal = 5,
}
