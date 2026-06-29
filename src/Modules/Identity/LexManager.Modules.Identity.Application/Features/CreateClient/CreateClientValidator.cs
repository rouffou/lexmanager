using FluentValidation;
using LexManager.Modules.Identity.Domain.Clients;

namespace LexManager.Modules.Identity.Application.Features.CreateClient;

/// <summary>
/// Syntactic validation (Normes §3.1). Conditional rules per client type; semantic invariants
/// (e.g. SIRET checksum semantics) stay in the domain value objects.
/// </summary>
public sealed class CreateClientValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.Type)
            .IsInEnum();

        When(command => command.Type == ClientType.PhysicalPerson, () =>
        {
            RuleFor(command => command.FirstName).NotEmpty();
            RuleFor(command => command.LastName).NotEmpty();
            RuleFor(command => command.NationalIdentityNumber).NotEmpty();
        });

        When(command => command.Type == ClientType.LegalPerson, () =>
        {
            RuleFor(command => command.CompanyName).NotEmpty();
            RuleFor(command => command.RegistrationNumber)
                .NotEmpty()
                .Must(siret => siret is not null && siret.Replace(" ", string.Empty).Length == 14)
                .WithMessage("The SIRET registration number must contain exactly 14 digits.");
        });
    }
}
