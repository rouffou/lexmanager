using FluentValidation;

namespace LexManager.Modules.Documents.Application.Features.UploadDocument;

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
