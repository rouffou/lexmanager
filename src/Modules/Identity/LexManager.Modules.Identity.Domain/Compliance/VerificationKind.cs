namespace LexManager.Modules.Identity.Domain.Compliance;

/// <summary>
/// The kind of identity/compliance verification carried out during client due diligence
/// (CNIE/passport, company registry extract, beneficial owners, PEP &amp; sanctions screening).
/// </summary>
public enum VerificationKind
{
    IdentityDocument = 1,
    AddressProof = 2,
    CompanyRegistry = 3,
    BeneficialOwner = 4,
    PepScreening = 5,
    SanctionsScreening = 6
}
