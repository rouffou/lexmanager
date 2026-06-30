using System.Net;
using System.Net.Http.Json;
using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.IntegrationTests.Infrastructure;
using LexManager.Modules.Calendar.Contracts;
using NSubstitute;

namespace LexManager.Modules.Billing.IntegrationTests;

[Collection(nameof(BillingApiCollection))]
public class BillingIntegrationTests(BillingApiFactory factory)
{
    private readonly BillingApiFactory _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [DockerFact]
    public async Task Invoice_Lifecycle_Draft_Issue_Pay()
    {
        Guid id = await CreateDraftAsync();

        // Add a line.
        HttpResponseMessage addLine = await _client.PostAsJsonAsync($"/api/billing/documents/{id}/lines",
            new { description = "Forfait dossier", quantity = 1m, unitPrice = 1500m });
        addLine.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Issue.
        HttpResponseMessage issue = await _client.PostAsJsonAsync($"/api/billing/documents/{id}/issue",
            new { dueDateUtc = DateTime.UtcNow.AddDays(30) });
        issue.StatusCode.Should().Be(HttpStatusCode.OK);

        BillingDocumentResponse? issued = await _client.GetFromJsonAsync<BillingDocumentResponse>($"/api/billing/documents/{id}");
        issued!.Status.Should().Be("Issued");
        issued.Number.Should().StartWith("FAC-");
        issued.Total.Should().Be(1800m); // 1500 + 20% VAT

        // Pay.
        HttpResponseMessage pay = await _client.PostAsync($"/api/billing/documents/{id}/payments", content: null);
        pay.StatusCode.Should().Be(HttpStatusCode.NoContent);

        BillingDocumentResponse? paid = await _client.GetFromJsonAsync<BillingDocumentResponse>($"/api/billing/documents/{id}");
        paid!.Status.Should().Be("Paid");
    }

    [DockerFact]
    public async Task GenerateTimeBasedInvoice_Should_UseBillableMinutes()
    {
        Guid caseId = Guid.NewGuid();
        _factory.TimeTrackingApi.GetCaseTimeSummaryAsync(caseId, Arg.Any<CancellationToken>())
            .Returns(new CaseTimeSummaryResponse(caseId, 120, 90, 2)); // 90 billable min => 1.5h

        var payload = new { caseId, clientId = Guid.NewGuid(), hourlyRate = 200m, dueDateUtc = DateTime.UtcNow.AddDays(30) };
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/billing/documents/time-based", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        Created? created = await response.Content.ReadFromJsonAsync<Created>();
        BillingDocumentResponse? document = await _client.GetFromJsonAsync<BillingDocumentResponse>($"/api/billing/documents/{created!.Id}");

        document!.Status.Should().Be("Issued");
        document.Mode.Should().Be("TimeBased");
        document.Subtotal.Should().Be(300m);
        document.Total.Should().Be(363m); // 21% Belgian VAT
    }

    private async Task<Guid> CreateDraftAsync()
    {
        var payload = new { caseId = Guid.NewGuid(), clientId = Guid.NewGuid(), kind = "Invoice", mode = "Flat", taxRatePercent = 20m };
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/billing/documents", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        Created? created = await response.Content.ReadFromJsonAsync<Created>();
        return created!.Id;
    }

    private sealed record Created(Guid Id);
}

[CollectionDefinition(nameof(BillingApiCollection))]
public sealed class BillingApiCollection : ICollectionFixture<BillingApiFactory>;
