namespace LexManager.Modules.Calendar.Domain.Common;

/// <summary>External calendar a local event is synchronised with (Microsoft Graph / Google).</summary>
public enum CalendarProvider
{
    None = 0,
    Microsoft = 1,
    Google = 2
}
