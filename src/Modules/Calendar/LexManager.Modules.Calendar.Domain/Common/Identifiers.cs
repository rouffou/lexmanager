namespace LexManager.Modules.Calendar.Domain.Common;

public readonly record struct CalendarEventId(Guid Value)
{
    public static CalendarEventId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct TimeEntryId(Guid Value)
{
    public static TimeEntryId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

/// <summary>Kind of agenda entry (SRD Module 4: audiences, délibérés, RDV, échéances).</summary>
public enum CalendarEventType
{
    Hearing = 1,
    Deliberation = 2,
    ClientAppointment = 3,
    ProcedureDeadline = 4,
    Other = 5
}

/// <summary>External calendar a local event is synchronised with (Microsoft Graph / Google).</summary>
public enum CalendarProvider
{
    None = 0,
    Microsoft = 1,
    Google = 2
}

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
