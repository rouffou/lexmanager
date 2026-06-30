using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using LexManager.Modules.Documents.Contracts;
using LexManager.Modules.Documents.IntegrationTests.Infrastructure;

namespace LexManager.Modules.Documents.IntegrationTests;

[Collection(nameof(DocumentsApiCollection))]
public class DocumentLifecycleIntegrationTests(DocumentsApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [DockerFact]
    public async Task Upload_Then_Download_And_Version_Document()
    {
        Guid caseId = Guid.NewGuid();
        byte[] original = Encoding.UTF8.GetBytes("conclusions v1");

        Guid documentId = await UploadAsync(caseId, "conclusions.txt", original);

        DocumentResponse? document = await _client.GetFromJsonAsync<DocumentResponse>($"/api/documents/{documentId}");
        document.Should().NotBeNull();
        document!.CurrentVersion.Should().Be(1);
        document.CaseId.Should().Be(caseId);

        byte[] downloaded = await _client.GetByteArrayAsync($"/api/documents/{documentId}/content");
        downloaded.Should().Equal(original);

        // Add a second version.
        using var versionContent = new MultipartFormDataContent();
        var v2 = new ByteArrayContent(Encoding.UTF8.GetBytes("conclusions v2"));
        v2.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        versionContent.Add(v2, "file", "conclusions-v2.txt");

        HttpResponseMessage versionResponse = await _client.PostAsync($"/api/documents/{documentId}/versions", versionContent);
        versionResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        DocumentResponse? updated = await _client.GetFromJsonAsync<DocumentResponse>($"/api/documents/{documentId}");
        updated!.CurrentVersion.Should().Be(2);
        updated.Versions.Should().HaveCount(2);
    }

    [DockerFact]
    public async Task GenerateFromTemplate_Should_CreateGeneratedDocument()
    {
        Guid caseId = Guid.NewGuid();
        var payload = new
        {
            caseId,
            templateKey = "engagement-letter",
            values = new Dictionary<string, string>
            {
                ["ClientName"] = "Jean Dupont",
                ["CaseTitle"] = "Litige Commercial",
                ["City"] = "Paris",
                ["Date"] = "30/06/2026"
            }
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/documents/generate", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        Created? created = await response.Content.ReadFromJsonAsync<Created>();
        DocumentResponse? generated = await _client.GetFromJsonAsync<DocumentResponse>($"/api/documents/{created!.Id}");
        generated!.Category.Should().Be("Generated");
        generated.CaseId.Should().Be(caseId);
    }

    [DockerFact]
    public async Task GenerateFromTemplate_Should_Return404_ForUnknownTemplate()
    {
        var payload = new { caseId = Guid.NewGuid(), templateKey = "nope", values = new Dictionary<string, string>() };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/documents/generate", payload);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Guid> UploadAsync(Guid caseId, string fileName, byte[] content)
    {
        using var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(content);
        file.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        form.Add(file, "file", fileName);
        form.Add(new StringContent(caseId.ToString()), "caseId");
        form.Add(new StringContent("ProcedureDocument"), "category");
        form.Add(new StringContent("false"), "isConfidential");

        HttpResponseMessage response = await _client.PostAsync("/api/documents", form);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        Created? created = await response.Content.ReadFromJsonAsync<Created>();
        return created!.Id;
    }

    private sealed record Created(Guid Id);
}

[CollectionDefinition(nameof(DocumentsApiCollection))]
public sealed class DocumentsApiCollection : ICollectionFixture<DocumentsApiFactory>;
