using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.GenerateFromTemplate;

public sealed class GenerateFromTemplateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/documents/generate", async (
                GenerateFromTemplateCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/documents/{id}", new { id }));
            })
            .WithName("GenerateDocumentFromTemplate")
            .WithTags("Documents")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
