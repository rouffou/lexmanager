using FluentValidation;

namespace LexManager.Modules.Billing.Application.Features.GenerateTimeBasedInvoice;

public sealed class GenerateTimeBasedInvoiceValidator : AbstractValidator<GenerateTimeBasedInvoiceCommand>
{
    public GenerateTimeBasedInvoiceValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.ClientId).NotEmpty();
        RuleFor(command => command.HourlyRate).GreaterThan(0m);
    }
}
