using LexManager.Modules.Billing.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace LexManager.Modules.Billing.Infrastructure.Reminders;

/// <summary>
/// Default reminder sender: records that a reminder would be sent. A production implementation
/// dispatches email/postal reminders for overdue payments (SRD Module 5: relances automatiques).
/// </summary>
internal sealed class LoggingPaymentReminderSender(ILogger<LoggingPaymentReminderSender> logger) : IPaymentReminderSender
{
    public Task SendAsync(Guid documentId, Guid clientId, decimal amount, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Payment reminder for document {DocumentId} to client {ClientId} for {Amount:0.00}.",
            documentId, clientId, amount);
        return Task.CompletedTask;
    }
}
