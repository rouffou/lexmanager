using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Carpa;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.OpenCarpaAccount;

public sealed class OpenCarpaAccountCommandHandler(
    ICarpaAccountRepository repository,
    ICaseApi caseApi,
    IClientApi clientApi,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<OpenCarpaAccountCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(OpenCarpaAccountCommand request, CancellationToken cancellationToken = default)
    {
        if (!await caseApi.CaseExistsAsync(request.CaseId, cancellationToken))
        {
            return Result.Failure<Guid>(CarpaErrors.CaseNotFound);
        }

        if (!await clientApi.ClientExistsAsync(request.ClientId, cancellationToken))
        {
            return Result.Failure<Guid>(CarpaErrors.ClientNotFound);
        }

        if (await repository.ExistsForCaseAsync(request.CaseId, cancellationToken))
        {
            return Result.Failure<Guid>(CarpaErrors.AlreadyExistsForCase);
        }

        CarpaAccount account = CarpaAccount.Open(request.CaseId, request.ClientId, request.Currency);

        repository.Add(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(account.Id.Value);
    }
}
