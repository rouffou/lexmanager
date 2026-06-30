using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.IssueBillingDocument;

public sealed class IssueBillingDocumentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/documents/{id:guid}/issue", async (
                Guid id,
                IssueBillingDocumentRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<string> result = await sender.Send(new IssueBillingDocumentCommand(id, body.DueDateUtc), cancellationToken);
                return result.ToApiResult(number => Results.Ok(new { number }));
            })
            .WithName("IssueBillingDocument")
            .WithTags("Billing")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
