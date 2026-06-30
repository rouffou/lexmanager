using FluentValidation;

namespace LexManager.Modules.Calendar.Application.Features.LogTimeEntry;

public sealed class LogTimeEntryValidator : AbstractValidator<LogTimeEntryCommand>
{
    public LogTimeEntryValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DurationMinutes).GreaterThan(0);
    }
}
