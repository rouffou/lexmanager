using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.GetCaseById;

public sealed class GetCaseByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/cases/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<CaseResponse> result = await sender.Send(new GetCaseByIdQuery(id), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetCaseById")
            .WithTags("Cases")
            .Produces<CaseResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
