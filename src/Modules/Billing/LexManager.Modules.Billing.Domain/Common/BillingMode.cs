namespace LexManager.Modules.Billing.Domain.Common;

/// <summary>How the fees are computed (SRD Module 5).</summary>
public enum BillingMode
{
    Flat = 1,         // au forfait
    TimeBased = 2,    // au temps passé (feuille de temps)
    SuccessFee = 3    // honoraire de résultat
}
