using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.CaseManagement.Domain.Cases;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.CaseManagement.Infrastructure.Persistence;

internal sealed class CaseReadRepository(CaseManagementDbContext context) : ICaseReadRepository
{
    public async Task<CaseResponse?> GetByIdAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        var id = new CaseId(caseId);

        Case? @case = await context.Cases
            .AsNoTracking()
            .Include(c => c.AdverseParties)
            .IgnoreQueryFilters() // a specific case is retrievable even once archived (traced access, SRD §5.3)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return @case is null ? null : MapFull(@case);
    }

    public async Task<PagedList<CaseSummaryResponse>> GetPagedAsync(
        PaginationParameters parameters,
        bool includeArchived,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Case> query = context.Cases.AsNoTracking();

        if (includeArchived)
        {
            query = query.IgnoreQueryFilters();
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<Case> cases = await query
            .OrderByDescending(c => c.OpenedOnUtc)
            .Skip(parameters.Skip)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        IReadOnlyList<CaseSummaryResponse> items = cases.Select(MapSummary).ToList();

        return new PagedList<CaseSummaryResponse>(items, parameters.Page, parameters.PageSize, totalCount);
    }

    private static CaseResponse MapFull(Case @case) => new(
        @case.Id.Value,
        @case.Title,
        @case.ClientId,
        @case.Status.ToString(),
        @case.IsArchived,
        @case.Jurisdiction is null
            ? null
            : new JurisdictionResponse(@case.Jurisdiction.CourtName, @case.Jurisdiction.GeneralRegisterNumber, @case.Jurisdiction.Judge),
        @case.AdverseParties.Select(party => new AdversePartyResponse(party.Name, party.Counsel)).ToList(),
        @case.OpenedOnUtc,
        @case.ClosedOnUtc);

    private static CaseSummaryResponse MapSummary(Case @case) => new(
        @case.Id.Value,
        @case.Title,
        @case.ClientId,
        @case.Status.ToString(),
        @case.OpenedOnUtc);
}
