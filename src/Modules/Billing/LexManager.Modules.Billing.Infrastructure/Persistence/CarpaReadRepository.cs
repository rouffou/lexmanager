using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.Domain.Carpa;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Billing.Infrastructure.Persistence;

internal sealed class CarpaReadRepository(BillingDbContext context) : ICarpaReadRepository
{
    public async Task<CarpaAccountResponse?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var id = new CarpaAccountId(accountId);

        CarpaAccount? account = await context.CarpaAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return account is null ? null : Map(account);
    }

    public async Task<CarpaAccountResponse?> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        CarpaAccount? account = await context.CarpaAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.CaseId == caseId, cancellationToken);

        return account is null ? null : Map(account);
    }

    private static CarpaAccountResponse Map(CarpaAccount account) => new(
        account.Id.Value,
        account.CaseId,
        account.ClientId,
        account.Currency,
        account.Balance.Amount,
        account.Transactions
            .OrderBy(transaction => transaction.OccurredOnUtc)
            .Select(transaction => new CarpaTransactionResponse(
                transaction.Type.ToString(),
                transaction.AmountValue,
                transaction.Description,
                transaction.Counterparty,
                transaction.OccurredOnUtc))
            .ToList(),
        account.OpenedOnUtc);
}
