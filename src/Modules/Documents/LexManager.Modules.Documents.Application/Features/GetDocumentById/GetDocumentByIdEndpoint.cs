using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Documents.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.GetDocumentById;

public sealed class GetDocumentByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/documents/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<DocumentResponse> result = await sender.Send(new GetDocumentByIdQuery(id), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetDocumentById")
            .WithTags("Documents")
            .Produces<DocumentResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
