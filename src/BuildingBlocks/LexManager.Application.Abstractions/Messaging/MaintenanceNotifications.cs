using Mediarq.Core.Common.Requests.Notifications;

namespace LexManager.Application.Abstractions.Messaging;

/// <summary>
/// Published on a schedule by the retention worker. Modules subscribe with an
/// <c>INotificationHandler</c> to purge/anonymise their own expired data (SRD §5.3) — keeping the
/// worker decoupled from each module's storage.
/// </summary>
public sealed record RetentionSweepRequested(DateTime AsOfUtc) : INotification;

/// <summary>
/// Published on a schedule to trigger overdue-payment review and reminders (SRD Module 5). The
/// Billing module handles it; the worker stays unaware of billing internals.
/// </summary>
public sealed record PaymentReminderSweepRequested(DateTime AsOfUtc) : INotification;
