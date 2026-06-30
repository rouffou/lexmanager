using LexManager.Modules.Billing.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.GetBillingDocument;

public sealed record GetBillingDocumentByIdQuery(Guid DocumentId) : IQuery<Result<BillingDocumentResponse>>;
