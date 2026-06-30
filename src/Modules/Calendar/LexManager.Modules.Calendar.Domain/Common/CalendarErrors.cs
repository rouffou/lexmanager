using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Domain.Common;

public static class CalendarErrors
{
    public static readonly ResultError EventNotFound = ResultError.NotFound(
        "Calendar.EventNotFound", "No calendar event was found for the supplied identifier.");

    public static readonly ResultError ScheduleConflict = ResultError.Conflict(
        "Calendar.ScheduleConflict", "The event overlaps another event already in the agenda.");

    public static readonly ResultError InvalidTimeRange = ResultError.Failure(
        "Calendar.InvalidTimeRange", "The event end must be after its start.");

    public static readonly ResultError EmptyTitle = ResultError.Failure(
        "Calendar.EmptyTitle", "A calendar event requires a title.");

    public static readonly ResultError CaseNotFound = ResultError.Problem(
        "Calendar.CaseNotFound", "The referenced case does not exist.");

    public static readonly ResultError InvalidDuration = ResultError.Failure(
        "TimeTracking.InvalidDuration", "The logged duration must be greater than zero.");

    public static readonly ResultError TimeEntryCaseRequired = ResultError.Failure(
        "TimeTracking.CaseRequired", "A time entry must be attached to a case.");
}
