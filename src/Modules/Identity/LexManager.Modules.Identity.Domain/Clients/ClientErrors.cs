using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Domain.Clients;

/// <summary>
/// Canonical, structured errors for the Clients aggregate. Returned via the Result pattern
/// or carried by domain exceptions, then mapped to RFC 7807 ProblemDetails at the boundary.
/// </summary>
public static class ClientErrors
{
    public static readonly ResultError ConflictOfInterestDetected = ResultError.Conflict(
        "Client.ConflictOfInterest",
        "A conflict of interest was detected: the firm already represents the opposing party.");

    public static readonly ResultError NotFound = ResultError.NotFound(
        "Client.NotFound",
        "No client was found for the supplied identifier.");

    public static readonly ResultError InvalidEmail = ResultError.Failure(
        "Client.InvalidEmail",
        "The email address is not in a valid format.");

    public static readonly ResultError MissingPersonName = ResultError.Failure(
        "Client.MissingPersonName",
        "A natural person requires both a first name and a last name.");

    public static readonly ResultError MissingCompanyName = ResultError.Failure(
        "Client.MissingCompanyName",
        "A legal entity requires a company name.");

    public static readonly ResultError InvalidSiret = ResultError.Failure(
        "Client.InvalidSiret",
        "The SIRET registration number must contain exactly 14 digits.");

    public static readonly ResultError MissingIdentityNumber = ResultError.Failure(
        "Client.MissingIdentityNumber",
        "A natural person requires a national identity number (CNIE).");
}
