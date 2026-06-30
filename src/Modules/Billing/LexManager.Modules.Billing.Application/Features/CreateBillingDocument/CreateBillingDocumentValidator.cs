using FluentValidation;

namespace LexManager.Modules.Billing.Application.Features.CreateBillingDocument;

public sealed class CreateBillingDocumentValidator : AbstractValidator<CreateBillingDocumentCommand>
{
    public CreateBillingDocumentValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.ClientId).NotEmpty();
        RuleFor(command => command.TaxRatePercent).InclusiveBetween(0m, 100m);
    }
}
