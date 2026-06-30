using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.RegisterPayment;

public sealed class RegisterPaymentCommandHandler(
    IBillingDocumentRepository repository,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<RegisterPaymentCommand, Result>
{
    public async Task<Result> Handle(RegisterPaymentCommand request, CancellationToken cancellationToken = default)
    {
        BillingDocument? document = await repository.GetByIdAsync(new BillingDocumentId(request.DocumentId), cancellationToken);
        if (document is null)
        {
            return Result.Failure(BillingErrors.NotFound);
        }

        if (document.Status is not (BillingStatus.Issued or BillingStatus.Overdue))
        {
            return Result.Failure(BillingErrors.NotIssued);
        }

        document.RegisterPayment();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
