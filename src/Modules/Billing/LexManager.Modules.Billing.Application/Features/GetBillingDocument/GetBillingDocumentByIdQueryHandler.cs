using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.GetBillingDocument;

public sealed class GetBillingDocumentByIdQueryHandler(IBillingReadRepository readRepository)
    : IQueryHandler<GetBillingDocumentByIdQuery, Result<BillingDocumentResponse>>
{
    public async Task<Result<BillingDocumentResponse>> Handle(GetBillingDocumentByIdQuery request, CancellationToken cancellationToken = default)
    {
        BillingDocumentResponse? document = await readRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        return document is null
            ? Result.Failure<BillingDocumentResponse>(BillingErrors.NotFound)
            : Result.Success(document);
    }
}
