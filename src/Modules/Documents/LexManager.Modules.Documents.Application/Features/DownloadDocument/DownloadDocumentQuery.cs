using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Contracts;
using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.DownloadDocument;

public sealed record DownloadDocumentQuery(Guid DocumentId, int? Version) : IQuery<Result<DocumentDownload>>;

public sealed class DownloadDocumentQueryHandler(
    IDocumentReadRepository readRepository,
    IDocumentStorage storage) : IQueryHandler<DownloadDocumentQuery, Result<DocumentDownload>>
{
    public async Task<Result<DocumentDownload>> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken = default)
    {
        DocumentFileRef? fileRef = await readRepository.GetFileRefAsync(request.DocumentId, request.Version, cancellationToken);
        if (fileRef is null)
        {
            return Result.Failure<DocumentDownload>(DocumentErrors.NotFound);
        }

        byte[] content = await storage.ReadAsync(fileRef.StorageKey, cancellationToken);
        return Result.Success(new DocumentDownload(fileRef.FileName, fileRef.ContentType, content));
    }
}

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
