using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.AddDocumentVersion;

public sealed class AddDocumentVersionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/documents/{id:guid}/versions", async (
                Guid id,
                IFormFile file,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                using var buffer = new MemoryStream();
                await file.CopyToAsync(buffer, cancellationToken);

                Result<int> result = await sender.Send(new AddDocumentVersionCommand(id, buffer.ToArray()), cancellationToken);
                return result.ToApiResult(version => Results.Ok(new { version }));
            })
            .WithName("AddDocumentVersion")
            .WithTags("Documents")
            .DisableAntiforgery()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
