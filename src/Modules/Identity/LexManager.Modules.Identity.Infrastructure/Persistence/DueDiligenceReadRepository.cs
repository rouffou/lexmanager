using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Contracts;
using LexManager.Modules.Identity.Domain.Compliance;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Identity.Infrastructure.Persistence;

internal sealed class DueDiligenceReadRepository(IdentityDbContext context) : IDueDiligenceReadRepository
{
    public async Task<DueDiligenceResponse?> GetByIdAsync(Guid dueDiligenceId, CancellationToken cancellationToken = default)
    {
        var id = new DueDiligenceId(dueDiligenceId);

        ClientDueDiligence? file = await context.DueDiligenceFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

        return file is null ? null : Map(file);
    }

    public async Task<DueDiligenceResponse?> GetByClientAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        ClientDueDiligence? file = await context.DueDiligenceFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.ClientId == clientId, cancellationToken);

        return file is null ? null : Map(file);
    }

    public async Task<bool> IsClientApprovedAsync(Guid clientId, CancellationToken cancellationToken = default) =>
        await context.DueDiligenceFiles
            .AsNoTracking()
            .AnyAsync(f => f.ClientId == clientId && f.Status == DueDiligenceStatus.Approved, cancellationToken);

    private static DueDiligenceResponse Map(ClientDueDiligence file) => new(
        file.Id.Value,
        file.ClientId,
        file.Status.ToString(),
        file.RiskLevel.ToString(),
        file.IsPoliticallyExposed,
        file.ComplianceScore,
        file.CanApprove,
        file.RequiredChecks.Select(kind => kind.ToString()).ToList(),
        file.Checks
            .OrderBy(check => check.RecordedOnUtc)
            .Select(check => new VerificationCheckResponse(
                check.Kind.ToString(), check.Reference, check.Cleared, check.Notes, check.RecordedOnUtc))
            .ToList(),
        file.OpenedOnUtc,
        file.DecidedOnUtc,
        file.DecisionReason);
}
