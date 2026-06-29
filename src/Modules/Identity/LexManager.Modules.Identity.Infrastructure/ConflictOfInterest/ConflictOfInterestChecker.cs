using LexManager.Modules.Identity.Domain.Clients;

namespace LexManager.Modules.Identity.Infrastructure.ConflictOfInterest;

/// <summary>
/// MVP conflict-of-interest checker. The full rule (SRD Module 1) cross-references the parties
/// recorded by the Case Management module: a conflict exists if the supplied identity already
/// appears as an opposing party in an open case. That check will be performed through Case
/// Management's public contract once the module is available — never by querying its database.
/// For now no conflicts are reported, keeping the seam explicit and unit-testable.
/// </summary>
internal sealed class ConflictOfInterestChecker : IConflictOfInterestChecker
{
    public Task<bool> HasConflictAsync(string identityKey, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);
}
