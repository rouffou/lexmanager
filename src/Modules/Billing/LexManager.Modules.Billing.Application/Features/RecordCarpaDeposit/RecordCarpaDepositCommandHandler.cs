using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Carpa;
using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.RecordCarpaDeposit;

public sealed class RecordCarpaDepositCommandHandler(
    ICarpaAccountRepository repository,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<RecordCarpaDepositCommand, Result>
{
    public async Task<Result> Handle(RecordCarpaDepositCommand request, CancellationToken cancellationToken = default)
    {
        CarpaAccount? account = await repository.GetByIdAsync(new CarpaAccountId(request.AccountId), cancellationToken);
        if (account is null)
        {
            return Result.Failure(CarpaErrors.NotFound);
        }

        account.Deposit(Money.Of(request.Amount, account.Currency), request.Description, request.Counterparty);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
