using LexManager.Modules.CaseManagement.Domain.Cases;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.CaseManagement.Infrastructure.Persistence;

internal sealed class CaseRepository(CaseManagementDbContext context) : ICaseRepository
{
    public Task<Case?> GetByIdAsync(CaseId id, CancellationToken cancellationToken = default) =>
        context.Cases
            .Include(@case => @case.AdverseParties)
            .IgnoreQueryFilters() // write side may operate on archived cases (e.g. retention)
            .FirstOrDefaultAsync(@case => @case.Id == id, cancellationToken);

    public void Add(Case @case) => context.Cases.Add(@case);
}
