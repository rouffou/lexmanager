using LexManager.Application.Abstractions.Pagination;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Identity.Application.Features.GetClients;

public sealed class GetClientsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/clients", async (
                ISender sender,
                CancellationToken cancellationToken,
                int page = 1,
                int pageSize = 25,
                string? search = null) =>
            {
                Result<PagedList<ClientSummaryResponse>> result =
                    await sender.Send(new GetClientsQuery(page, pageSize, search), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetClients")
            .WithTags("Clients")
            .Produces<PagedList<ClientSummaryResponse>>();
    }
}
