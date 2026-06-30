using FluentValidation;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Domain.Documents;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.UploadDocument;

public sealed record UploadDocumentCommand(
    Guid CaseId,
    string FileName,
    string ContentType,
    DocumentCategory Category,
    bool IsConfidential,
    byte[] Content) : ICommand<Result<Guid>>;

public sealed class UploadDocumentValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.FileName).NotEmpty().MaximumLength(512);
        RuleFor(command => command.Content).NotNull().Must(content => content.Length > 0)
            .WithMessage("The uploaded document is empty.");
    }
}

public sealed class UploadDocumentCommandHandler(
    IDocumentRepository documentRepository,
    ICaseApi caseApi,
    IDocumentStorage storage,
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

        documentRepository.Add(document);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(document.Id.Value);
    }
}
