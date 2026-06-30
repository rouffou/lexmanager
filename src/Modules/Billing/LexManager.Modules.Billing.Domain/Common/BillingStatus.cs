namespace LexManager.Modules.Billing.Domain.Common;

/// <summary>Payment lifecycle (SRD Module 5: suivi des règlements).</summary>
public enum BillingStatus
{
    Draft = 1,
    Issued = 2,
    Paid = 3,
    Overdue = 4,
    Cancelled = 5
}
