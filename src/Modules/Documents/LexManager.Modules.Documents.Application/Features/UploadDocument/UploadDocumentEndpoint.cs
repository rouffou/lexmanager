using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.UploadDocument;

public sealed class UploadDocumentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/documents", async (
                IFormFile file,
                [FromForm] Guid caseId,
                [FromForm] DocumentCategory category,
                [FromForm] bool isConfidential,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                using var buffer = new MemoryStream();
                await file.CopyToAsync(buffer, cancellationToken);

                var command = new UploadDocumentCommand(
                    caseId, file.FileName, file.ContentType, category, isConfidential, buffer.ToArray());

                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/documents/{id}", new { id }));
            })
            .WithName("UploadDocument")
            .WithTags("Documents")
            .DisableAntiforgery()
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }
}
