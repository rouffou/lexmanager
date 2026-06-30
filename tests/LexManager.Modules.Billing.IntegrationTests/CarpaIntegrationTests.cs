using System.Net;
using System.Net.Http.Json;
using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.IntegrationTests.Infrastructure;

namespace LexManager.Modules.Billing.IntegrationTests;

[Collection(nameof(BillingApiCollection))]
public class CarpaIntegrationTests(BillingApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [DockerFact]
    public async Task Carpa_Lifecycle_Open_Deposit_Disburse_Trace()
    {
        Guid caseId = Guid.NewGuid();
        Guid clientId = Guid.NewGuid();

        // Open the third-party account.
        HttpResponseMessage open = await _client.PostAsJsonAsync("/api/billing/carpa/accounts",
            new { caseId, clientId, currency = "EUR" });
        open.StatusCode.Should().Be(HttpStatusCode.Created);
        Guid accountId = (await open.Content.ReadFromJsonAsync<Created>())!.Id;

        // Deposit a provision.
        HttpResponseMessage deposit = await _client.PostAsJsonAsync($"/api/billing/carpa/accounts/{accountId}/deposits",
            new { amount = 1000m, description = "Provision client", counterparty = "Client" });
        deposit.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Disburse part of it.
        HttpResponseMessage disburse = await _client.PostAsJsonAsync($"/api/billing/carpa/accounts/{accountId}/disbursements",
            new { amount = 400m, description = "Versement partie adverse", counterparty = "Partie adverse" });
        disburse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Balance and full movement trace.
        CarpaAccountResponse? account = await _client.GetFromJsonAsync<CarpaAccountResponse>($"/api/billing/carpa/accounts/{accountId}");
        account!.Balance.Should().Be(600m);
        account.Transactions.Should().HaveCount(2);
        account.Transactions[0].Type.Should().Be("Deposit");
        account.Transactions[1].Type.Should().Be("Disbursement");

        // Lookup by case returns the same account.
        CarpaAccountResponse? byCase = await _client.GetFromJsonAsync<CarpaAccountResponse>($"/api/billing/carpa/cases/{caseId}/account");
        byCase!.Id.Should().Be(accountId);
    }

    [DockerFact]
    public async Task Carpa_Disbursement_BeyondBalance_Should_Return_Conflict()
    {
        HttpResponseMessage open = await _client.PostAsJsonAsync("/api/billing/carpa/accounts",
            new { caseId = Guid.NewGuid(), clientId = Guid.NewGuid(), currency = "EUR" });
        Guid accountId = (await open.Content.ReadFromJsonAsync<Created>())!.Id;

        await _client.PostAsJsonAsync($"/api/billing/carpa/accounts/{accountId}/deposits",
            new { amount = 100m, description = "Petite provision", counterparty = (string?)null });

        HttpResponseMessage disburse = await _client.PostAsJsonAsync($"/api/billing/carpa/accounts/{accountId}/disbursements",
            new { amount = 250m, description = "Trop", counterparty = (string?)null });

        disburse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [DockerFact]
    public async Task Carpa_Open_Twice_ForSameCase_Should_Return_Conflict()
    {
        Guid caseId = Guid.NewGuid();

        HttpResponseMessage first = await _client.PostAsJsonAsync("/api/billing/carpa/accounts",
            new { caseId, clientId = Guid.NewGuid(), currency = "EUR" });
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        HttpResponseMessage second = await _client.PostAsJsonAsync("/api/billing/carpa/accounts",
            new { caseId, clientId = Guid.NewGuid(), currency = "EUR" });
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private sealed record Created(Guid Id);
}
