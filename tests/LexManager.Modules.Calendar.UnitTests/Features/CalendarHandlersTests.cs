using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Application.Features.LogTimeEntry;
using LexManager.Modules.Calendar.Application.Features.ScheduleEvent;
using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.Calendar.Domain.TimeTracking;
using LexManager.Modules.CaseManagement.Contracts;
using NSubstitute;

namespace LexManager.Modules.Calendar.UnitTests.Features;

public class CalendarHandlersTests
{
    private static readonly DateTime Start = new(2026, 6, 30, 9, 0, 0, DateTimeKind.Utc);

    private readonly ICalendarEventRepository _events = Substitute.For<ICalendarEventRepository>();
    private readonly ITimeEntryRepository _timeEntries = Substitute.For<ITimeEntryRepository>();
    private readonly ICaseApi _caseApi = Substitute.For<ICaseApi>();
    private readonly ICalendarSyncProvider _sync = Substitute.For<ICalendarSyncProvider>();
    private readonly ICalendarUnitOfWork _unitOfWork = Substitute.For<ICalendarUnitOfWork>();

    [Fact]
    public async Task ScheduleEvent_Should_ReturnConflict_WhenOverlapping()
    {
        _events.GetOverlappingAsync(Arg.Any<Guid>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>())
            .Returns([CalendarEvent.Schedule(Guid.NewGuid(), "Other", CalendarEventType.Other,
                DateRange.Create(Start, Start.AddHours(1)))]);

        var handler = new ScheduleEventCommandHandler(_events, _caseApi, _sync, _unitOfWork);
        var command = new ScheduleEventCommand(Guid.NewGuid(), "Audience", CalendarEventType.Hearing,
            Start, Start.AddHours(1));

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CalendarErrors.ScheduleConflict);
        _events.DidNotReceive().Add(Arg.Any<CalendarEvent>());
    }

    [Fact]
    public async Task ScheduleEvent_Should_Persist_WhenNoConflict()
    {
        _events.GetOverlappingAsync(Arg.Any<Guid>(), Arg.Any<DateRange>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _sync.PushAsync(Arg.Any<CalendarSyncRequest>(), Arg.Any<CancellationToken>())
            .Returns((CalendarSyncLink?)null);

        var handler = new ScheduleEventCommandHandler(_events, _caseApi, _sync, _unitOfWork);
        var command = new ScheduleEventCommand(Guid.NewGuid(), "Audience", CalendarEventType.Hearing,
            Start, Start.AddHours(1));

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _events.Received(1).Add(Arg.Any<CalendarEvent>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LogTimeEntry_Should_Fail_WhenCaseMissing()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var handler = new LogTimeEntryCommandHandler(_timeEntries, _caseApi, _unitOfWork);
        var result = await handler.Handle(
            new LogTimeEntryCommand(Guid.NewGuid(), Guid.NewGuid(), "Recherche", 30), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CalendarErrors.CaseNotFound);
        _timeEntries.DidNotReceive().Add(Arg.Any<TimeEntry>());
    }

    [Fact]
    public async Task LogTimeEntry_Should_Persist_WhenCaseExists()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        var handler = new LogTimeEntryCommandHandler(_timeEntries, _caseApi, _unitOfWork);
        var result = await handler.Handle(
            new LogTimeEntryCommand(Guid.NewGuid(), Guid.NewGuid(), "Recherche", 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _timeEntries.Received(1).Add(Arg.Is<TimeEntry>(entry => entry.DurationMinutes == 30));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
