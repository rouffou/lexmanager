using Mediarq.Core.Common.Requests.Notifications;

namespace LexManager.Application.Abstractions.Messaging;

/// <summary>
/// Published on a schedule by the retention worker. Modules subscribe with an
/// <c>INotificationHandler</c> to purge/anonymise their own expired data (SRD §5.3) — keeping the
/// worker decoupled from each module's storage.
/// </summary>
public sealed record RetentionSweepRequested(DateTime AsOfUtc) : INotification;
