namespace LexManager.Modules.Billing.Application.Abstractions;

/// <summary>Sends a payment reminder for an overdue document (SRD Module 5: relances automatiques).</summary>
public interface IPaymentReminderSender
{
    Task SendAsync(Guid documentId, Guid clientId, decimal amount, CancellationToken cancellationToken = default);
}
