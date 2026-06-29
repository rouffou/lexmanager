namespace LexManager.Modules.Identity.Domain.Clients;

/// <summary>
/// Performs the mandatory global conflict-of-interest search required before a client is created
/// (SRD Module 1): ensures the firm is not already acting for the opposing party. The concrete
/// implementation may consult other modules' public contracts (e.g. Case Management) — never their
/// database directly.
/// </summary>
public interface IConflictOfInterestChecker
{
    Task<bool> HasConflictAsync(string identityKey, CancellationToken cancellationToken = default);
}
