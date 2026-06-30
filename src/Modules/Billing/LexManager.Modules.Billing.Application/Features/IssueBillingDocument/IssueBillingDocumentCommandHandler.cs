using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.IssueBillingDocument;

public sealed class IssueBillingDocumentCommandHandler(
    IBillingDocumentRepository repository,
    IInvoiceNumberGenerator numberGenerator,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<IssueBillingDocumentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(IssueBillingDocumentCommand request, CancellationToken cancellationToken = default)
    {
        BillingDocument? document = await repository.GetByIdAsync(new BillingDocumentId(request.DocumentId), cancellationToken);
        if (document is null)
        {
            return Result.Failure<string>(BillingErrors.NotFound);
        }

        if (document.Status != BillingStatus.Draft)
        {
            return Result.Failure<string>(BillingErrors.NotEditable);
        }

        if (document.Lines.Count == 0)
        {
            return Result.Failure<string>(BillingErrors.CannotIssueEmpty);
        }

        string number = await numberGenerator.NextAsync(document.Kind, cancellationToken);
        document.Issue(number, request.DueDateUtc);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(number);
    }
}
