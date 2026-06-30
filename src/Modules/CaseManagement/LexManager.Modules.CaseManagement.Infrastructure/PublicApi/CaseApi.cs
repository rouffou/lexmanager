using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.CaseManagement.Domain.Cases;
using LexManager.Modules.CaseManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.CaseManagement.Infrastructure.PublicApi;

/// <summary>
/// Case Management's cross-module contract implementation. Used (for example) by the Identity
/// module's conflict-of-interest checker to ask whether the firm already acts against a party.
/// </summary>
internal sealed class CaseApi(CaseManagementDbContext context) : ICaseApi
{
    public Task<bool> CaseExistsAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        var id = new CaseId(caseId);
        return context.Cases.AsNoTracking().IgnoreQueryFilters().AnyAsync(@case => @case.Id == id, cancellationToken);
    }

    public Task<bool> HasOpenCaseAgainstAsync(string adversePartyName, CancellationToken cancellationToken = default) =>
        context.Cases
            .AsNoTracking()
            .Where(@case => @case.Status != CaseStatus.Closed)
            .SelectMany(@case => @case.AdverseParties)
            .AnyAsync(party => party.Name == adversePartyName, cancellationToken);
}
