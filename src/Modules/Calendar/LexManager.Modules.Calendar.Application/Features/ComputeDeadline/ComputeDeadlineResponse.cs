namespace LexManager.Modules.Calendar.Application.Features.ComputeDeadline;

public sealed record ComputeDeadlineResponse(DateOnly DueDate, Guid? ScheduledEventId);
