namespace LexManager.Modules.Billing.Domain.Common;

/// <summary>The kind of financial document issued (SRD Module 5).</summary>
public enum BillingDocumentKind
{
    Invoice = 1,        // facture
    ProvisionCall = 2,  // appel de provision
    FeeNote = 3         // note d'honoraires
}
