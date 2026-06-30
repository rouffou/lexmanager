namespace LexManager.Modules.Calendar.Domain.Common;

/// <summary>
/// Common procedural time limits used to compute legal deadlines (calcul des délais légaux).
/// Values are illustrative defaults; the calculator rolls the due date to the next business day.
/// </summary>
public enum LegalDeadlineType
{
    AppealAgainstJudgment = 1,   // appel d'un jugement — 1 mois
    OppositionDefault = 2,       // opposition — 1 mois
    AppealAgainstOrder = 3,      // appel d'une ordonnance — 15 jours
    Cassation = 4                // pourvoi en cassation — 2 mois
}
