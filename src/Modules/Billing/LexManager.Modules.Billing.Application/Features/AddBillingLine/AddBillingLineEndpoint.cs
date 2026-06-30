using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.AddBillingLine;

public sealed class AddBillingLineEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/documents/{id:guid}/lines", async (
                Guid id,
                AddBillingLineRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(
                    new AddBillingLineCommand(id, body.Description, body.Quantity, body.UnitPrice), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("AddBillingLine")
            .WithTags("Billing")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
