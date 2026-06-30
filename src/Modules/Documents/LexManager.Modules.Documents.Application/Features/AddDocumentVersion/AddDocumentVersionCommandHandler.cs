using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.AddDocumentVersion;

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
