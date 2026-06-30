using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.CreateBillingDocument;

public sealed class CreateBillingDocumentCommandHandler(
    IBillingDocumentRepository repository,
    ICaseApi caseApi,
    IClientApi clientApi,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<CreateBillingDocumentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBillingDocumentCommand request, CancellationToken cancellationToken = default)
    {
        if (!await caseApi.CaseExistsAsync(request.CaseId, cancellationToken))
        {
            return Result.Failure<Guid>(BillingErrors.CaseNotFound);
        }

        if (!await clientApi.ClientExistsAsync(request.ClientId, cancellationToken))
        {
            return Result.Failure<Guid>(BillingErrors.ClientNotFound);
        }

        BillingDocument document = BillingDocument.CreateDraft(
            request.CaseId, request.ClientId, request.Kind, request.Mode, request.TaxRatePercent, request.Currency, request.Regime);

        repository.Add(document);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(document.Id.Value);
    }
}
