using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.UploadDocument;

public sealed class UploadDocumentCommandHandler(
    IDocumentRepository documentRepository,
    ICaseApi caseApi,
    IDocumentStorage storage,
    IOcrTextExtractor ocr,
    IDocumentUnitOfWork unitOfWork) : ICommandHandler<UploadDocumentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken = default)
    {
        if (request.Content is null || request.Content.Length == 0)
        {
            return Result.Failure<Guid>(DocumentErrors.EmptyContent);
        }

        if (!await caseApi.CaseExistsAsync(request.CaseId, cancellationToken))
        {
            return Result.Failure<Guid>(DocumentErrors.CaseNotFound);
        }

        StoredFile stored = await storage.SaveAsync(request.Content, cancellationToken);

        Document document = Document.Create(
            request.CaseId,
            request.FileName,
            string.IsNullOrWhiteSpace(request.ContentType) ? "application/octet-stream" : request.ContentType,
            request.Category,
            stored.StorageKey,
            stored.SizeBytes,
            stored.Checksum,
            request.IsConfidential);

        await IndexContentAsync(document, request.Content, cancellationToken);

        documentRepository.Add(document);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(document.Id.Value);
    }

    private async Task IndexContentAsync(Document document, byte[] content, CancellationToken cancellationToken)
    {
        if (!ocr.CanExtract(document.ContentType))
        {
            return;
        }

        string text = await ocr.ExtractTextAsync(content, document.ContentType, cancellationToken);
        document.AttachExtractedText(text);
    }
}
