using LexManager.Modules.Calendar.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace LexManager.Modules.Calendar.Infrastructure.Sync;

/// <summary>
/// Default sync provider used until Microsoft Graph / Google Calendar credentials are configured.
/// It performs no external calls, so local events stay purely internal. Real providers implement
/// <see cref="ICalendarSyncProvider"/> to push events (masking private details) and reconcile webhooks.
/// </summary>
internal sealed class NoOpCalendarSyncProvider(ILogger<NoOpCalendarSyncProvider> logger) : ICalendarSyncProvider
{
    public Task<CalendarSyncLink?> PushAsync(CalendarSyncRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult<CalendarSyncLink?>(null);

    public Task HandleWebhookAsync(string provider, string payload, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Received calendar webhook from {Provider} ({Bytes} bytes); no external sync provider configured.",
            provider, payload.Length);
        return Task.CompletedTask;
    }
}
