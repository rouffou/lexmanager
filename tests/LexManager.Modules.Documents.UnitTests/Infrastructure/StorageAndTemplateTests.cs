using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Infrastructure.Storage;
using LexManager.Modules.Documents.Infrastructure.Templates;

namespace LexManager.Modules.Documents.UnitTests.Infrastructure;

public class StorageAndTemplateTests
{
    [Fact]
    public async Task FileSystemStorage_Should_RoundTrip_ContentAndChecksum()
    {
        string root = Path.Combine(Path.GetTempPath(), "lex-doc-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var storage = new FileSystemDocumentStorage(root);
            byte[] content = [10, 20, 30, 40, 50];

            StoredFile stored = await storage.SaveAsync(content);
            byte[] readBack = await storage.ReadAsync(stored.StorageKey);

            readBack.Should().Equal(content);
            stored.SizeBytes.Should().Be(5);
            stored.Checksum.Should().NotBeNullOrWhiteSpace();

            await storage.DeleteAsync(stored.StorageKey);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [Fact]
    public async Task TemplateRenderer_Should_SubstitutePlaceholders()
    {
        var renderer = new SimpleTemplateRenderer();
        var values = new Dictionary<string, string>
        {
            ["ClientName"] = "Jean Dupont",
            ["CaseTitle"] = "Litige Commercial",
            ["City"] = "Paris",
            ["Date"] = "30/06/2026"
        };

        RenderedTemplate? rendered = await renderer.RenderAsync("engagement-letter", values);

        rendered.Should().NotBeNull();
        string body = System.Text.Encoding.UTF8.GetString(rendered!.Content);
        body.Should().Contain("Jean Dupont").And.Contain("Litige Commercial");
        body.Should().NotContain("{{");
    }

    [Fact]
    public async Task TemplateRenderer_Should_ReturnNull_ForUnknownTemplate()
    {
        var renderer = new SimpleTemplateRenderer();

        RenderedTemplate? rendered = await renderer.RenderAsync("does-not-exist", new Dictionary<string, string>());

        rendered.Should().BeNull();
    }
}
