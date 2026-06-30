using LexManager.Application.Abstractions.Messaging;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Mediators;

namespace LexManager.Modules.Billing.Application.Features.ProcessOverdue;

/// <summary>
/// Reacts to the scheduled maintenance notification by running the overdue-payment review. This is
/// how the cross-cutting retention worker triggers billing reminders without depending on Billing.
/// </summary>
public sealed class PaymentReminderSweepHandler(ISender sender) : INotificationHandler<PaymentReminderSweepRequested>
{
    public async Task Handle(PaymentReminderSweepRequested notification, CancellationToken cancellationToken = default) =>
        await sender.Send(new ProcessOverdueCommand(), cancellationToken);
}
