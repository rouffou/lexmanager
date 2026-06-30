namespace LexManager.Modules.Billing.Domain.Common;

/// <summary>VAT regime applied to the document (SRD V11 §5: TVA belge 21% + exemptions).</summary>
public enum VatRegime
{
    Standard = 1,                      // TVA belge 21%
    ProDeo = 2,                        // aide juridique / pro deo — exonéré
    IntraCommunityReverseCharge = 3,   // autoliquidation intracommunautaire
    Exempt = 4                         // autre exonération
}
