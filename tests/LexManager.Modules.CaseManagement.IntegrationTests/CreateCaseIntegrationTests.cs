using System.Net;
using System.Net.Http.Json;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.CaseManagement.IntegrationTests.Infrastructure;

namespace LexManager.Modules.CaseManagement.IntegrationTests;

[Collection(nameof(CaseApiCollection))]
public class CreateCaseIntegrationTests(CaseApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [DockerFact]
    public async Task CreateCase_Should_PersistAndExposeLifecycle()
    {
        var payload = new
        {
            title = "Litige Commercial SNE",
            clientId = Guid.NewGuid(),
            courtName = "Tribunal judiciaire de Paris",
            generalRegisterNumber = "RG-2026-0042"
        };

        HttpResponseMessage create = await _client.PostAsJsonAsync("/api/cases", payload);
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        CreatedCase? created = await create.Content.ReadFromJsonAsync<CreatedCase>();
        created.Should().NotBeNull();

        CaseResponse? fetched = await _client.GetFromJsonAsync<CaseResponse>($"/api/cases/{created!.Id}");
        fetched.Should().NotBeNull();
        fetched!.Title.Should().Be("Litige Commercial SNE");
        fetched.Status.Should().Be("Opened");
        fetched.Jurisdiction!.GeneralRegisterNumber.Should().Be("RG-2026-0042");

        HttpResponseMessage close = await _client.PutAsync($"/api/cases/{created.Id}/close", content: null);
        close.StatusCode.Should().Be(HttpStatusCode.NoContent);

        CaseResponse? afterClose = await _client.GetFromJsonAsync<CaseResponse>($"/api/cases/{created.Id}");
        afterClose!.Status.Should().Be("Closed");
    }

    [DockerFact]
    public async Task CreateCase_Should_Return400_WhenTitleMissing()
    {
        var payload = new { title = "", clientId = Guid.NewGuid() };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/cases", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private sealed record CreatedCase(Guid Id);
}

[CollectionDefinition(nameof(CaseApiCollection))]
public sealed class CaseApiCollection : ICollectionFixture<CaseApiFactory>;
