namespace LexManager.Modules.CaseManagement.Domain.Procedures;

/// <summary>
/// Encodes the ordered milestone blueprint for each <see cref="ProcedureType"/> — the domain
/// knowledge behind the interactive procedure tree (SRD V11 §36). Sequences follow Belgian civil
/// procedure, from the amicable phase through to forced execution.
/// </summary>
public static class ProcedureCatalog
{
    private const string Amiable = "Phase amiable";
    private const string Introduction = "Introduction";
    private const string Instruction = "Mise en état";
    private const string Hearing = "Audience";
    private const string Judgment = "Jugement";
    private const string Enforcement = "Exécution";
    private const string Appeals = "Voies de recours";

    private static readonly IReadOnlyList<ProcedureStageBlueprint> DebtRecovery =
    [
        new(1, "Mise en demeure", Amiable),
        new(2, "Citation / requête en paiement", Introduction),
        new(3, "Audience d'introduction", Introduction),
        new(4, "Conclusions", Instruction),
        new(5, "Plaidoiries", Hearing),
        new(6, "Délibéré", Judgment),
        new(7, "Jugement", Judgment),
        new(8, "Signification du jugement", Enforcement),
        new(9, "Exécution forcée (saisie)", Enforcement),
    ];

    private static readonly IReadOnlyList<ProcedureStageBlueprint> CivilLitigation =
    [
        new(1, "Consultation & analyse", Amiable),
        new(2, "Mise en demeure", Amiable),
        new(3, "Citation / requête contradictoire", Introduction),
        new(4, "Audience d'introduction", Introduction),
        new(5, "Calendrier de mise en état (art. 747 C.J.)", Instruction),
        new(6, "Conclusions", Instruction),
        new(7, "Plaidoiries", Hearing),
        new(8, "Délibéré", Judgment),
        new(9, "Jugement", Judgment),
        new(10, "Signification & voies de recours", Appeals),
    ];

    private static readonly IReadOnlyList<ProcedureStageBlueprint> SummaryProceedings =
    [
        new(1, "Requête en référé", Introduction),
        new(2, "Audience de référé", Hearing),
        new(3, "Ordonnance", Judgment),
        new(4, "Signification", Enforcement),
    ];

    private static readonly IReadOnlyList<ProcedureStageBlueprint> LabourDispute =
    [
        new(1, "Mise en demeure", Amiable),
        new(2, "Requête contradictoire", Introduction),
        new(3, "Comparution & conciliation", Introduction),
        new(4, "Conclusions", Instruction),
        new(5, "Plaidoiries", Hearing),
        new(6, "Jugement", Judgment),
    ];

    private static readonly IReadOnlyList<ProcedureStageBlueprint> Appeal =
    [
        new(1, "Déclaration d'appel", Introduction),
        new(2, "Constitution d'avocat", Introduction),
        new(3, "Conclusions d'appel", Instruction),
        new(4, "Plaidoiries", Hearing),
        new(5, "Arrêt", Judgment),
    ];

    public static IReadOnlyList<ProcedureStageBlueprint> For(ProcedureType type) => type switch
    {
        ProcedureType.DebtRecovery => DebtRecovery,
        ProcedureType.CivilLitigation => CivilLitigation,
        ProcedureType.SummaryProceedings => SummaryProceedings,
        ProcedureType.LabourDispute => LabourDispute,
        ProcedureType.Appeal => Appeal,
        _ => [],
    };
}
