using System.Net;
using System.Net.Http.Json;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.CaseManagement.IntegrationTests.Infrastructure;

namespace LexManager.Modules.CaseManagement.IntegrationTests;

[Collection(nameof(CaseApiCollection))]
public class ProcedureIntegrationTests(CaseApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [DockerFact]
    public async Task ProcedureTree_Should_Generate_Advance_And_Schedule()
    {
        Guid caseId = await CreateCaseAsync();

        // Generate the tree.
        HttpResponseMessage generate = await _client.PostAsJsonAsync(
            $"/api/cases/{caseId}/procedure",
            new { procedureType = "DebtRecovery", referenceOnUtc = "2026-01-15T00:00:00Z" });
        generate.StatusCode.Should().Be(HttpStatusCode.Created);

        GeneratedPlan? created = await generate.Content.ReadFromJsonAsync<GeneratedPlan>();
        created.Should().NotBeNull();

        ProcedurePlanResponse? plan = await _client.GetFromJsonAsync<ProcedurePlanResponse>($"/api/cases/{caseId}/procedure");
        plan.Should().NotBeNull();
        plan!.Type.Should().Be("DebtRecovery");
        plan.Stages.Should().HaveCount(9);
        plan.ProgressPercent.Should().Be(0);
        plan.CurrentStageOrder.Should().Be(1);
        plan.Stages[0].Status.Should().Be("Current");

        // Advance the current stage.
        HttpResponseMessage advance = await _client.PostAsync($"/api/cases/procedure/{created!.Id}/advance", content: null);
        advance.StatusCode.Should().Be(HttpStatusCode.NoContent);

        ProcedurePlanResponse? advanced = await _client.GetFromJsonAsync<ProcedurePlanResponse>($"/api/cases/{caseId}/procedure");
        advanced!.CurrentStageOrder.Should().Be(2);
        advanced.ProgressPercent.Should().BeGreaterThan(0);
        advanced.Stages[0].Status.Should().Be("Completed");

        // Schedule a downstream stage.
        HttpResponseMessage schedule = await _client.PostAsJsonAsync(
            $"/api/cases/procedure/{created.Id}/stages/3/schedule",
            new { plannedOnUtc = "2026-03-01T00:00:00Z" });
        schedule.StatusCode.Should().Be(HttpStatusCode.NoContent);

        ProcedurePlanResponse? scheduled = await _client.GetFromJsonAsync<ProcedurePlanResponse>($"/api/cases/{caseId}/procedure");
        scheduled!.Stages.Single(stage => stage.Order == 3).PlannedOnUtc.Should().NotBeNull();
    }

    [DockerFact]
    public async Task Generate_Should_Return409_WhenPlanAlreadyExists()
    {
        Guid caseId = await CreateCaseAsync();
        var payload = new { procedureType = "SummaryProceedings", referenceOnUtc = "2026-02-01T00:00:00Z" };

        HttpResponseMessage first = await _client.PostAsJsonAsync($"/api/cases/{caseId}/procedure", payload);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        HttpResponseMessage second = await _client.PostAsJsonAsync($"/api/cases/{caseId}/procedure", payload);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private async Task<Guid> CreateCaseAsync()
    {
        HttpResponseMessage create = await _client.PostAsJsonAsync(
            "/api/cases",
            new { title = "Dossier procédure", clientId = Guid.NewGuid() });
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        GeneratedPlan? created = await create.Content.ReadFromJsonAsync<GeneratedPlan>();
        return created!.Id;
    }

    private sealed record GeneratedPlan(Guid Id);
}
