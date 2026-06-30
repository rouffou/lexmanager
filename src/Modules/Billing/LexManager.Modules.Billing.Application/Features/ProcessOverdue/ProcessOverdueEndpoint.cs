using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.ProcessOverdue;

public sealed class ProcessOverdueEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/process-overdue", async (ISender sender, CancellationToken cancellationToken) =>
            {
                Result<int> result = await sender.Send(new ProcessOverdueCommand(), cancellationToken);
                return result.ToApiResult(count => Results.Ok(new { remindersSent = count }));
            })
            .WithName("ProcessOverdueBilling")
            .WithTags("Billing")
            .Produces(StatusCodes.Status200OK);
    }
}
