using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.Domain.Carpa;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.GetCarpaAccount;

public sealed class GetCarpaAccountByIdQueryHandler(ICarpaReadRepository readRepository)
    : IQueryHandler<GetCarpaAccountByIdQuery, Result<CarpaAccountResponse>>
{
    public async Task<Result<CarpaAccountResponse>> Handle(GetCarpaAccountByIdQuery request, CancellationToken cancellationToken = default)
    {
        CarpaAccountResponse? account = await readRepository.GetByIdAsync(request.AccountId, cancellationToken);
        return account is null
            ? Result.Failure<CarpaAccountResponse>(CarpaErrors.NotFound)
            : Result.Success(account);
    }
}
