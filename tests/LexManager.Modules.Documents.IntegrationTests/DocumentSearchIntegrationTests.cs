using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Documents.Contracts;
using LexManager.Modules.Documents.IntegrationTests.Infrastructure;

namespace LexManager.Modules.Documents.IntegrationTests;

[Collection(nameof(DocumentsApiCollection))]
public class DocumentSearchIntegrationTests(DocumentsApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [DockerFact]
    public async Task Search_Should_FindDocument_ByExtractedBody()
    {
        Guid caseId = Guid.NewGuid();
        await UploadAsync(caseId, "note-bail.txt", "Le contrat de bail commercial arrive à échéance en septembre.");
        await UploadAsync(caseId, "note-divorce.txt", "Requête conjointe en divorce déposée au greffe.");

        PagedList<DocumentSearchResultResponse>? results =
            await _client.GetFromJsonAsync<PagedList<DocumentSearchResultResponse>>(
                $"/api/documents/search?q=bail&caseId={caseId}");

        results.Should().NotBeNull();
        results!.Items.Should().ContainSingle();
        results.Items[0].FileName.Should().Be("note-bail.txt");
        results.Items[0].Highlight.Should().Contain("bail");
    }

    [DockerFact]
    public async Task Search_Should_MatchOnFileName()
    {
        Guid caseId = Guid.NewGuid();
        await UploadAsync(caseId, "assignation-tribunal.txt", "Contenu sans le mot recherché dans le corps.");

        PagedList<DocumentSearchResultResponse>? results =
            await _client.GetFromJsonAsync<PagedList<DocumentSearchResultResponse>>(
                $"/api/documents/search?q=assignation&caseId={caseId}");

        results!.Items.Should().Contain(r => r.FileName == "assignation-tribunal.txt");
    }

    [DockerFact]
    public async Task Search_Should_ReturnEmpty_WhenNoMatch()
    {
        Guid caseId = Guid.NewGuid();
        await UploadAsync(caseId, "quelconque.txt", "Un texte anodin sans terme particulier.");

        PagedList<DocumentSearchResultResponse>? results =
            await _client.GetFromJsonAsync<PagedList<DocumentSearchResultResponse>>(
                $"/api/documents/search?q=hypothèque&caseId={caseId}");

        results!.Items.Should().BeEmpty();
        results.TotalCount.Should().Be(0);
    }

    private async Task UploadAsync(Guid caseId, string fileName, string body)
    {
        using var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(Encoding.UTF8.GetBytes(body));
        file.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        form.Add(file, "file", fileName);
        form.Add(new StringContent(caseId.ToString()), "caseId");
        form.Add(new StringContent("ProcedureDocument"), "category");
        form.Add(new StringContent("false"), "isConfidential");

        HttpResponseMessage response = await _client.PostAsync("/api/documents", form);
        response.EnsureSuccessStatusCode();
    }
}
