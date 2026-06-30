using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.RegisterPayment;

public sealed class RegisterPaymentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/documents/{id:guid}/payments", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(new RegisterPaymentCommand(id), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("RegisterPayment")
            .WithTags("Billing")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
