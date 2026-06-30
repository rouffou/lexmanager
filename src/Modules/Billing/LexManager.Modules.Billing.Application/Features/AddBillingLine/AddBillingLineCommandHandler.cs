using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.AddBillingLine;

public sealed class AddBillingLineCommandHandler(
    IBillingDocumentRepository repository,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<AddBillingLineCommand, Result>
{
    public async Task<Result> Handle(AddBillingLineCommand request, CancellationToken cancellationToken = default)
    {
        BillingDocument? document = await repository.GetByIdAsync(new BillingDocumentId(request.DocumentId), cancellationToken);
        if (document is null)
        {
            return Result.Failure(BillingErrors.NotFound);
        }

        document.AddLine(request.Description, request.Quantity, Money.Of(request.UnitPrice, document.Currency));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
