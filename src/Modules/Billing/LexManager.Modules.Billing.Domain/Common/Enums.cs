namespace LexManager.Modules.Billing.Domain.Common;

public readonly record struct BillingDocumentId(Guid Value)
{
    public static BillingDocumentId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

/// <summary>The kind of financial document issued (SRD Module 5).</summary>
public enum BillingDocumentKind
{
    Invoice = 1,        // facture
    ProvisionCall = 2,  // appel de provision
    FeeNote = 3         // note d'honoraires
}

/// <summary>How the fees are computed (SRD Module 5).</summary>
public enum BillingMode
{
    Flat = 1,         // au forfait
    TimeBased = 2,    // au temps passé (feuille de temps)
    SuccessFee = 3    // honoraire de résultat
}

/// <summary>Payment lifecycle (SRD Module 5: suivi des règlements).</summary>
public enum BillingStatus
{
    Draft = 1,
    Issued = 2,
    Paid = 3,
    Overdue = 4,
    Cancelled = 5
}
