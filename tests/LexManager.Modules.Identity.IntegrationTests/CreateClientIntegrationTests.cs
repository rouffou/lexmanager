using System.Net;
using System.Net.Http.Json;
using LexManager.Modules.Identity.Contracts;
using LexManager.Modules.Identity.IntegrationTests.Infrastructure;

namespace LexManager.Modules.Identity.IntegrationTests;

[Collection(nameof(IdentityApiCollection))]
public class CreateClientIntegrationTests(IdentityApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [DockerFact]
    public async Task CreateClient_Should_PersistAndBeRetrievable()
    {
        var payload = new
        {
            type = "PhysicalPerson",
            email = "client.integration@example.com",
            firstName = "Camille",
            lastName = "Durand",
            nationalIdentityNumber = "CNIE-INT-001"
        };

        HttpResponseMessage createResponse = await _client.PostAsJsonAsync("/api/clients", payload);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        CreatedClient? created = await createResponse.Content.ReadFromJsonAsync<CreatedClient>();
        created.Should().NotBeNull();
        created!.Id.Should().NotBe(Guid.Empty);

        ClientResponse? fetched = await _client.GetFromJsonAsync<ClientResponse>($"/api/clients/{created.Id}");
        fetched.Should().NotBeNull();
        fetched!.DisplayName.Should().Be("Camille Durand");
        fetched.Email.Should().Be("client.integration@example.com");
        fetched.Type.Should().Be("PhysicalPerson");
    }

    [DockerFact]
    public async Task CreateClient_Should_Return400_WhenPayloadInvalid()
    {
        var payload = new { type = "PhysicalPerson", email = "not-an-email" };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/clients", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [DockerFact]
    public async Task GetClientById_Should_Return404_WhenMissing()
    {
        HttpResponseMessage response = await _client.GetAsync($"/api/clients/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private sealed record CreatedClient(Guid Id);
}

[CollectionDefinition(nameof(IdentityApiCollection))]
public sealed class IdentityApiCollection : ICollectionFixture<IdentityApiFactory>;
