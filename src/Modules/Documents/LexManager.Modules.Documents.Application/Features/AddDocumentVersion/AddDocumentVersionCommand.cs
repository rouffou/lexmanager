using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.AddDocumentVersion;

public sealed record AddDocumentVersionCommand(Guid DocumentId, byte[] Content) : ICommand<Result<int>>;

public sealed class AddDocumentVersionCommandHandler(
    IDocumentRepository documentRepository,
    IDocumentStorage storage,
    IDocumentUnitOfWork unitOfWork) : ICommandHandler<AddDocumentVersionCommand, Result<int>>
{
    public async Task<Result<int>> Handle(AddDocumentVersionCommand request, CancellationToken cancellationToken = default)
    {
        if (request.Content is null || request.Content.Length == 0)
        {
            return Result.Failure<int>(DocumentErrors.EmptyContent);
        }

        Document? document = await documentRepository.GetByIdAsync(new DocumentId(request.DocumentId), cancellationToken);
        if (document is null)
        {
            return Result.Failure<int>(DocumentErrors.NotFound);
        }

        StoredFile stored = await storage.SaveAsync(request.Content, cancellationToken);
        DocumentVersion version = document.AddVersion(stored.StorageKey, stored.SizeBytes, stored.Checksum);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(version.VersionNumber);
    }
}

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
