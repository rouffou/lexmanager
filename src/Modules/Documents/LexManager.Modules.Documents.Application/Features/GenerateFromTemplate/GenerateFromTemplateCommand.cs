using FluentValidation;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.GenerateFromTemplate;

/// <summary>Generates a document from a predefined template (publipostage / mail merge, SRD Module 3).</summary>
public sealed record GenerateFromTemplateCommand(
    Guid CaseId,
    string TemplateKey,
    Dictionary<string, string> Values) : ICommand<Result<Guid>>;

public sealed class GenerateFromTemplateValidator : AbstractValidator<GenerateFromTemplateCommand>
{
    public GenerateFromTemplateValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.TemplateKey).NotEmpty();
    }
}

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

public sealed class GenerateFromTemplateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/documents/generate", async (
                GenerateFromTemplateCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/documents/{id}", new { id }));
            })
            .WithName("GenerateDocumentFromTemplate")
            .WithTags("Documents")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
