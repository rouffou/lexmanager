using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.SimulateLegalInterest;

public sealed class SimulateLegalInterestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/legal-interest/simulate", async (
                SimulateLegalInterestQuery query,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<LegalInterestResponse> result = await sender.Send(query, cancellationToken);
                return result.ToApiResult();
            })
            .WithName("SimulateLegalInterest")
            .WithTags("Billing")
            .Produces<LegalInterestResponse>()
            .ProducesValidationProblem();
    }
}
