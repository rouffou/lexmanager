using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.ComputeDeadline;

public sealed class ComputeDeadlineEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/calendar/deadlines/compute", async (
                ComputeDeadlineCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<ComputeDeadlineResponse> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult();
            })
            .WithName("ComputeDeadline")
            .WithTags("Calendar")
            .Produces<ComputeDeadlineResponse>();
    }
}
