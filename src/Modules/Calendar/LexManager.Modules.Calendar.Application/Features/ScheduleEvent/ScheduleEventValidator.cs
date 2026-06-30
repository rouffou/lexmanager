using FluentValidation;

namespace LexManager.Modules.Calendar.Application.Features.ScheduleEvent;

public sealed class ScheduleEventValidator : AbstractValidator<ScheduleEventCommand>
{
    public ScheduleEventValidator()
    {
        RuleFor(command => command.OwnerUserId).NotEmpty();
        RuleFor(command => command.Title).NotEmpty().MaximumLength(256);
        RuleFor(command => command.EndUtc).GreaterThan(command => command.StartUtc);
    }
}
