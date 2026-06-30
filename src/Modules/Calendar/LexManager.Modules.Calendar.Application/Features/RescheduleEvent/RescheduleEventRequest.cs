namespace LexManager.Modules.Calendar.Application.Features.RescheduleEvent;

public sealed record RescheduleEventRequest(DateTime StartUtc, DateTime EndUtc);
