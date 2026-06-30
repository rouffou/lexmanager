namespace LexManager.Modules.Calendar.Application.Abstractions;

/// <summary>
/// Bidirectional synchronisation with an external calendar (Microsoft Graph / Google Calendar).
/// The default implementation is a no-op; real providers push events, honour the
/// <see cref="CalendarSyncRequest.MaskDetails"/> flag, and feed webhook callbacks back in (SRD Module 4).
/// </summary>
public interface ICalendarSyncProvider
{
    Task<CalendarSyncLink?> PushAsync(CalendarSyncRequest request, CancellationToken cancellationToken = default);

    Task HandleWebhookAsync(string provider, string payload, CancellationToken cancellationToken = default);
}
