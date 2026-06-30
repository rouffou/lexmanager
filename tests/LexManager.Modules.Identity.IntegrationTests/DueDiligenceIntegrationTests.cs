using System.Net;
using System.Net.Http.Json;
using LexManager.Modules.Identity.Contracts;
using LexManager.Modules.Identity.IntegrationTests.Infrastructure;

namespace LexManager.Modules.Identity.IntegrationTests;

[Collection(nameof(IdentityApiCollection))]
public class DueDiligenceIntegrationTests(IdentityApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [DockerFact]
    public async Task Kyc_Lifecycle_Start_Verify_Score_Approve()
    {
        Guid clientId = await CreatePhysicalClientAsync();

        // Start the due-diligence file.
        HttpResponseMessage start = await _client.PostAsJsonAsync(
            $"/api/clients/{clientId}/due-diligence", new { riskLevel = "Standard", isPoliticallyExposed = false });
        start.StatusCode.Should().Be(HttpStatusCode.Created);
        Guid fileId = (await start.Content.ReadFromJsonAsync<Created>())!.Id;

        // Initially incomplete.
        DueDiligenceResponse? pending = await _client.GetFromJsonAsync<DueDiligenceResponse>($"/api/clients/{clientId}/due-diligence");
        pending!.Status.Should().Be("InProgress");
        pending.ComplianceScore.Should().Be(0);
        pending.CanApprove.Should().BeFalse();
        pending.RequiredChecks.Should().BeEquivalentTo(["IdentityDocument", "AddressProof"]);

        // Clear the two mandatory checks.
        await _client.PostAsJsonAsync($"/api/clients/due-diligence/{fileId}/checks",
            new { kind = "IdentityDocument", reference = "PASS-INT-1", cleared = true, notes = (string?)null });
        await _client.PostAsJsonAsync($"/api/clients/due-diligence/{fileId}/checks",
            new { kind = "AddressProof", reference = "BILL-INT-1", cleared = true, notes = (string?)null });

        DueDiligenceResponse? scored = await _client.GetFromJsonAsync<DueDiligenceResponse>($"/api/clients/{clientId}/due-diligence");
        scored!.ComplianceScore.Should().Be(100);
        scored.CanApprove.Should().BeTrue();

        // Accept the mandate.
        HttpResponseMessage approve = await _client.PostAsJsonAsync(
            $"/api/clients/due-diligence/{fileId}/decision", new { approve = true, reason = (string?)null });
        approve.StatusCode.Should().Be(HttpStatusCode.NoContent);

        DueDiligenceResponse? approved = await _client.GetFromJsonAsync<DueDiligenceResponse>($"/api/clients/{clientId}/due-diligence");
        approved!.Status.Should().Be("Approved");

        // A second decision is rejected — the file is already decided.
        HttpResponseMessage again = await _client.PostAsJsonAsync(
            $"/api/clients/due-diligence/{fileId}/decision", new { approve = true, reason = (string?)null });
        again.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [DockerFact]
    public async Task Kyc_Approve_WithIncompleteChecks_Should_Return_Conflict()
    {
        Guid clientId = await CreatePhysicalClientAsync();

        HttpResponseMessage start = await _client.PostAsJsonAsync(
            $"/api/clients/{clientId}/due-diligence", new { riskLevel = "Standard", isPoliticallyExposed = false });
        Guid fileId = (await start.Content.ReadFromJsonAsync<Created>())!.Id;

        // Only one of the two required checks cleared → score 50.
        await _client.PostAsJsonAsync($"/api/clients/due-diligence/{fileId}/checks",
            new { kind = "IdentityDocument", reference = "PASS-INT-2", cleared = true, notes = (string?)null });

        HttpResponseMessage approve = await _client.PostAsJsonAsync(
            $"/api/clients/due-diligence/{fileId}/decision", new { approve = true, reason = (string?)null });

        approve.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [DockerFact]
    public async Task Kyc_Start_Twice_ForSameClient_Should_Return_Conflict()
    {
        Guid clientId = await CreatePhysicalClientAsync();

        HttpResponseMessage first = await _client.PostAsJsonAsync(
            $"/api/clients/{clientId}/due-diligence", new { riskLevel = "Standard", isPoliticallyExposed = false });
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        HttpResponseMessage second = await _client.PostAsJsonAsync(
            $"/api/clients/{clientId}/due-diligence", new { riskLevel = "Standard", isPoliticallyExposed = false });
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private async Task<Guid> CreatePhysicalClientAsync()
    {
        var payload = new
        {
            type = "PhysicalPerson",
            email = $"kyc-{Guid.NewGuid():N}@example.com",
            firstName = "Kyc",
            lastName = "Client",
            nationalIdentityNumber = $"CNIE-{Guid.NewGuid():N}"
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/clients", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<Created>())!.Id;
    }

    private sealed record Created(Guid Id);
}
