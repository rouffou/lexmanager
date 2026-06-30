using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.CreateBillingDocument;

public sealed class CreateBillingDocumentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/documents", async (CreateBillingDocumentCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/billing/documents/{id}", new { id }));
            })
            .WithName("CreateBillingDocument")
            .WithTags("Billing")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }
}
