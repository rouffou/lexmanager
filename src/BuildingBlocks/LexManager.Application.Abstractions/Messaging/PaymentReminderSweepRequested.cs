using Mediarq.Core.Common.Requests.Notifications;

namespace LexManager.Application.Abstractions.Messaging;

/// <summary>
/// Published on a schedule to trigger overdue-payment review and reminders (SRD Module 5). The
/// Billing module handles it; the worker stays unaware of billing internals.
/// </summary>
public sealed record PaymentReminderSweepRequested(DateTime AsOfUtc) : INotification;
