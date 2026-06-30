using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Domain.Compliance;

/// <summary>Canonical errors for the client due-diligence (KYC / LCB-FT) workflow.</summary>
public static class KycErrors
{
    public static readonly ResultError NotFound = ResultError.NotFound(
        "Kyc.NotFound", "No due-diligence file was found for the supplied identifier.");

    public static readonly ResultError ClientNotFound = ResultError.Problem(
        "Kyc.ClientNotFound", "The referenced client does not exist.");

    public static readonly ResultError AlreadyExistsForClient = ResultError.Conflict(
        "Kyc.AlreadyExistsForClient", "A due-diligence file already exists for this client.");

    public static readonly ResultError AlreadyDecided = ResultError.Conflict(
        "Kyc.AlreadyDecided", "The due-diligence file has already been decided and can no longer be changed.");

    public static readonly ResultError IncompleteDueDiligence = ResultError.Conflict(
        "Kyc.IncompleteDueDiligence",
        "The mandate cannot be accepted: required vigilance checks are not all cleared (compliance score below 100).");

    public static readonly ResultError MissingReference = ResultError.Failure(
        "Kyc.MissingReference", "A verification check requires a document or screening reference.");

    public static readonly ResultError MissingRejectionReason = ResultError.Failure(
        "Kyc.MissingRejectionReason", "Rejecting a due-diligence file requires a reason.");
}
