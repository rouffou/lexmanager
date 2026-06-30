using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.GenerateFromTemplate;

public sealed class GenerateFromTemplateCommandHandler(
    IDocumentRepository documentRepository,
    ICaseApi caseApi,
    ITemplateRenderer templateRenderer,
    IDocumentStorage storage,
    IDocumentUnitOfWork unitOfWork) : ICommandHandler<GenerateFromTemplateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(GenerateFromTemplateCommand request, CancellationToken cancellationToken = default)
    {
        if (!await caseApi.CaseExistsAsync(request.CaseId, cancellationToken))
        {
            return Result.Failure<Guid>(DocumentErrors.CaseNotFound);
        }

        RenderedTemplate? rendered = await templateRenderer.RenderAsync(
            request.TemplateKey,
            request.Values ?? new Dictionary<string, string>(),
            cancellationToken);

        if (rendered is null)
        {
            return Result.Failure<Guid>(DocumentErrors.TemplateNotFound);
        }

        StoredFile stored = await storage.SaveAsync(rendered.Content, cancellationToken);

        Document document = Document.Create(
            request.CaseId,
            rendered.FileName,
            rendered.ContentType,
            DocumentCategory.Generated,
            stored.StorageKey,
            stored.SizeBytes,
            stored.Checksum);

        documentRepository.Add(document);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(document.Id.Value);
    }
}
