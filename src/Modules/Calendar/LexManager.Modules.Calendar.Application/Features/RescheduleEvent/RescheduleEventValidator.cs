using FluentValidation;

namespace LexManager.Modules.Calendar.Application.Features.RescheduleEvent;

public sealed class RescheduleEventValidator : AbstractValidator<RescheduleEventCommand>
{
    public RescheduleEventValidator() => RuleFor(command => command.EndUtc).GreaterThan(command => command.StartUtc);
}
