using System.Net;
using System.Net.Http.Json;
using LexManager.Modules.Calendar.Contracts;
using LexManager.Modules.Calendar.IntegrationTests.Infrastructure;

namespace LexManager.Modules.Calendar.IntegrationTests;

[Collection(nameof(CalendarApiCollection))]
public class CalendarIntegrationTests(CalendarApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [DockerFact]
    public async Task ScheduleEvent_Then_Conflict_Then_Agenda()
    {
        Guid owner = Guid.NewGuid();
        DateTime start = new(2026, 9, 1, 9, 0, 0, DateTimeKind.Utc);

        var first = new
        {
            ownerUserId = owner,
            title = "Audience TGI",
            type = "Hearing",
            startUtc = start,
            endUtc = start.AddHours(2)
        };

        HttpResponseMessage create = await _client.PostAsJsonAsync("/api/calendar/events", first);
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        // Overlapping event for the same owner -> conflict.
        var overlapping = new
        {
            ownerUserId = owner,
            title = "RDV client",
            type = "ClientAppointment",
            startUtc = start.AddHours(1),
            endUtc = start.AddHours(3)
        };
        HttpResponseMessage conflict = await _client.PostAsJsonAsync("/api/calendar/events", overlapping);
        conflict.StatusCode.Should().Be(HttpStatusCode.Conflict);

        // Agenda for the day returns the first event.
        string from = Uri.EscapeDataString(start.AddHours(-1).ToString("o"));
        string to = Uri.EscapeDataString(start.AddHours(6).ToString("o"));
        var agenda = await _client.GetFromJsonAsync<PagedAgenda>(
            $"/api/calendar/events?ownerUserId={owner}&from={from}&to={to}");

        agenda!.TotalCount.Should().Be(1);
        agenda.Items.Should().ContainSingle(e => e.Title == "Audience TGI");
    }

    [DockerFact]
    public async Task LogTime_Then_Summary_RoundsToQuarterHour()
    {
        Guid caseId = Guid.NewGuid();

        await _client.PostAsJsonAsync("/api/time-entries", new
        {
            caseId,
            userId = Guid.NewGuid(),
            description = "Rédaction conclusions",
            durationMinutes = 20,   // -> 30
            isBillable = true
        });
        await _client.PostAsJsonAsync("/api/time-entries", new
        {
            caseId,
            userId = Guid.NewGuid(),
            description = "Appel client",
            durationMinutes = 10,   // -> 15
            isBillable = false
        });

        var summary = await _client.GetFromJsonAsync<CaseTimeSummaryResponse>($"/api/time-entries/summary?caseId={caseId}");

        summary!.TotalMinutes.Should().Be(45);
        summary.BillableMinutes.Should().Be(30);
        summary.EntryCount.Should().Be(2);
    }

    [DockerFact]
    public async Task ComputeDeadline_Should_ReturnDueDate()
    {
        var payload = new { baseDate = "2026-06-30", type = "AppealAgainstJudgment" };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/calendar/deadlines/compute", payload);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DeadlineResult? result = await response.Content.ReadFromJsonAsync<DeadlineResult>();
        result!.DueDate.Should().Be(new DateOnly(2026, 7, 30));
    }

    private sealed record PagedAgenda(IReadOnlyList<CalendarEventSummaryResponse> Items, int TotalCount);
    private sealed record DeadlineResult(DateOnly DueDate, Guid? ScheduledEventId);
}

[CollectionDefinition(nameof(CalendarApiCollection))]
public sealed class CalendarApiCollection : ICollectionFixture<CalendarApiFactory>;
