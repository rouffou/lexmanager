using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Documents.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.DownloadDocument;

public sealed class DownloadDocumentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/documents/{id:guid}/content", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken,
                int? version = null) =>
            {
                Result<DocumentDownload> result = await sender.Send(new DownloadDocumentQuery(id, version), cancellationToken);
                if (result.IsFailure)
                {
                    return ApiResults.Problem(result.Error);
                }

                DocumentDownload download = result.Value;
                return Results.File(download.Content, download.ContentType, download.FileName);
            })
            .WithName("DownloadDocument")
            .WithTags("Documents")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
