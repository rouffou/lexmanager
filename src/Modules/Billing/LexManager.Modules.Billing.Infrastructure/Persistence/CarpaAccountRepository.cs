using LexManager.Modules.Billing.Domain.Carpa;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Billing.Infrastructure.Persistence;

internal sealed class CarpaAccountRepository(BillingDbContext context) : ICarpaAccountRepository
{
    public Task<CarpaAccount?> GetByIdAsync(CarpaAccountId id, CancellationToken cancellationToken = default) =>
        context.CarpaAccounts.FirstOrDefaultAsync(account => account.Id == id, cancellationToken);

    public Task<CarpaAccount?> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default) =>
        context.CarpaAccounts.FirstOrDefaultAsync(account => account.CaseId == caseId, cancellationToken);

    public Task<bool> ExistsForCaseAsync(Guid caseId, CancellationToken cancellationToken = default) =>
        context.CarpaAccounts.AnyAsync(account => account.CaseId == caseId, cancellationToken);

    public void Add(CarpaAccount account) => context.CarpaAccounts.Add(account);
}
