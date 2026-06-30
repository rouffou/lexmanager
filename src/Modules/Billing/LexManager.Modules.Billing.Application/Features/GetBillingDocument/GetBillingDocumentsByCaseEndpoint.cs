using LexManager.Application.Abstractions.Pagination;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.GetBillingDocument;

public sealed class GetBillingDocumentsByCaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/billing/documents", async (
                Guid caseId,
                ISender sender,
                CancellationToken cancellationToken,
                int page = 1,
                int pageSize = 25) =>
            {
                Result<PagedList<BillingDocumentSummaryResponse>> result =
                    await sender.Send(new GetBillingDocumentsByCaseQuery(caseId, page, pageSize), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetBillingDocumentsByCase")
            .WithTags("Billing")
            .Produces<PagedList<BillingDocumentSummaryResponse>>();
    }
}
