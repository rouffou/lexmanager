using LexManager.Application.Abstractions.Messaging;

namespace LexManager.Modules.CaseManagement.Contracts;

/// <summary>
/// Case Management's public contract. Notably lets the Identity module run its conflict-of-interest
/// search (is the firm already acting against this party?) without touching this module's database.
/// </summary>
public interface ICaseApi : IModuleApi
{
    Task<bool> CaseExistsAsync(Guid caseId, CancellationToken cancellationToken = default);

    Task<bool> HasOpenCaseAgainstAsync(string adversePartyName, CancellationToken cancellationToken = default);
}
