namespace LexManager.Modules.Calendar.Domain.Common;

/// <summary>Kind of agenda entry (SRD Module 4: audiences, délibérés, RDV, échéances).</summary>
public enum CalendarEventType
{
    Hearing = 1,
    Deliberation = 2,
    ClientAppointment = 3,
    ProcedureDeadline = 4,
    Other = 5
}
