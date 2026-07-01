using FluentValidation;

namespace LexManager.Modules.Documents.Application.Features.SearchDocuments;

public sealed class SearchDocumentsValidator : AbstractValidator<SearchDocumentsQuery>
{
    public SearchDocumentsValidator()
    {
        RuleFor(query => query.Term)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("The search term must be at least 2 characters long.");
    }
}
