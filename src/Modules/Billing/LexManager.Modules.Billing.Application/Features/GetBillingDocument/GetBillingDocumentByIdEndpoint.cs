using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.GetBillingDocument;

public sealed class GetBillingDocumentByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/billing/documents/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<BillingDocumentResponse> result = await sender.Send(new GetBillingDocumentByIdQuery(id), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetBillingDocumentById")
            .WithTags("Billing")
            .Produces<BillingDocumentResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
