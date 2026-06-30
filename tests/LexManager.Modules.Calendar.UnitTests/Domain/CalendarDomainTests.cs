using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Deadlines;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.Calendar.Domain.TimeTracking;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Calendar.UnitTests.Domain;

public class CalendarDomainTests
{
    private static readonly DateTime Base = new(2026, 6, 30, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void DateRange_Create_Should_Throw_WhenEndNotAfterStart()
    {
        Action act = () => DateRange.Create(Base, Base);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(CalendarErrors.InvalidTimeRange);
    }

    [Theory]
    [InlineData(0, 60, 30, 90, true)]    // overlapping
    [InlineData(0, 60, 60, 120, false)]  // touching boundaries do not overlap
    [InlineData(0, 60, 120, 180, false)] // disjoint
    public void DateRange_Overlaps_Works(int aStart, int aEnd, int bStart, int bEnd, bool expected)
    {
        DateRange a = DateRange.Create(Base.AddMinutes(aStart), Base.AddMinutes(aEnd));
        DateRange b = DateRange.Create(Base.AddMinutes(bStart), Base.AddMinutes(bEnd));

        a.Overlaps(b).Should().Be(expected);
    }

    [Fact]
    public void CalendarEvent_Schedule_Should_RaiseEvent()
    {
        CalendarEvent calendarEvent = CalendarEvent.Schedule(
            Guid.NewGuid(), "Audience", CalendarEventType.Hearing,
            DateRange.Create(Base, Base.AddHours(1)));

        calendarEvent.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CalendarEventScheduledDomainEvent>();
    }

    [Theory]
    [InlineData(1, 15)]
    [InlineData(15, 15)]
    [InlineData(16, 30)]
    [InlineData(46, 60)]
    public void TimeEntry_Should_RoundUpToQuarterHour(int raw, int expected)
    {
        TimeEntry.RoundUpToQuarter(raw).Should().Be(expected);
    }

    [Fact]
    public void TimeEntry_Log_Should_RoundAndRaiseEvent()
    {
        TimeEntry entry = TimeEntry.Log(Guid.NewGuid(), Guid.NewGuid(), "Recherche", 20);

        entry.DurationMinutes.Should().Be(30);
        entry.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<TimeEntryLoggedDomainEvent>();
    }

    [Fact]
    public void TimeEntry_Log_Should_Throw_OnZeroDuration()
    {
        Action act = () => TimeEntry.Log(Guid.NewGuid(), Guid.NewGuid(), "x", 0);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(CalendarErrors.InvalidDuration);
    }

    [Fact]
    public void DeadlineCalculator_AppealAgainstOrder_Adds15Days_AndAvoidsWeekend()
    {
        // 2026-06-30 + 15 days = 2026-07-15 (Wednesday).
        DateOnly due = LegalDeadlineCalculator.Compute(new DateOnly(2026, 6, 30), LegalDeadlineType.AppealAgainstOrder);

        due.Should().Be(new DateOnly(2026, 7, 15));
        due.DayOfWeek.Should().NotBe(DayOfWeek.Saturday).And.NotBe(DayOfWeek.Sunday);
    }

    [Fact]
    public void DeadlineCalculator_Should_RollWeekendToMonday()
    {
        // 2026-07-03 (Friday) + 1 day window check: pick a base whose +15d lands on Saturday.
        // 2026-06-27 is a Saturday; +15 days = 2026-07-12 (Sunday) -> rolled to Monday 2026-07-13.
        DateOnly due = LegalDeadlineCalculator.Compute(new DateOnly(2026, 6, 27), LegalDeadlineType.AppealAgainstOrder);

        due.DayOfWeek.Should().Be(DayOfWeek.Monday);
    }
}
