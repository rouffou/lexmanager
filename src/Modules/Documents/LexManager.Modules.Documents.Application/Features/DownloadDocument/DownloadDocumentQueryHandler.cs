using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Contracts;
using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.DownloadDocument;

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
