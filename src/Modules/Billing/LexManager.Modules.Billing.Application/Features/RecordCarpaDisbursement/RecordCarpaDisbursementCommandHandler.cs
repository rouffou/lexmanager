using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Carpa;
using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.RecordCarpaDisbursement;

public sealed class RecordCarpaDisbursementCommandHandler(
    ICarpaAccountRepository repository,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<RecordCarpaDisbursementCommand, Result>
{
    public async Task<Result> Handle(RecordCarpaDisbursementCommand request, CancellationToken cancellationToken = default)
    {
        CarpaAccount? account = await repository.GetByIdAsync(new CarpaAccountId(request.AccountId), cancellationToken);
        if (account is null)
        {
            return Result.Failure(CarpaErrors.NotFound);
        }

        // No overdraft allowed on third-party funds — branch on the outcome rather than throwing.
        if (request.Amount > account.Balance.Amount)
        {
            return Result.Failure(CarpaErrors.InsufficientFunds);
        }

        account.Disburse(Money.Of(request.Amount, account.Currency), request.Description, request.Counterparty);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
