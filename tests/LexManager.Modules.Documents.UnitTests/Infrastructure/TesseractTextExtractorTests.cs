using System.Text;
using LexManager.Modules.Documents.Infrastructure.Ocr;
using Microsoft.Extensions.Logging.Abstractions;

namespace LexManager.Modules.Documents.UnitTests.Infrastructure;

public class TesseractTextExtractorTests
{
    private static TesseractTextExtractor Create(string executablePath = "tesseract") =>
        new(new TesseractOptions { ExecutablePath = executablePath, Languages = "fra+nld", Timeout = TimeSpan.FromSeconds(5) },
            NullLogger<TesseractTextExtractor>.Instance);

    [Theory]
    [InlineData("text/plain", true)]
    [InlineData("text/plain; charset=utf-8", true)]
    [InlineData("IMAGE/PNG", true)]
    [InlineData("image/jpeg", true)]
    [InlineData("application/pdf", false)]
    [InlineData("application/zip", false)]
    [InlineData("", false)]
    public void CanExtract_Should_MatchSupportedTypes(string contentType, bool expected)
    {
        Create().CanExtract(contentType).Should().Be(expected);
    }

    [Fact]
    public async Task ExtractTextAsync_Should_DecodePlainText()
    {
        byte[] content = Encoding.UTF8.GetBytes("  Conclusions en demande — article 700  ");

        string text = await Create().ExtractTextAsync(content, "text/plain");

        text.Should().Be("Conclusions en demande — article 700");
    }

    [Fact]
    public async Task ExtractTextAsync_Should_StripUtf8Bom()
    {
        byte[] bom = [0xEF, 0xBB, 0xBF];
        byte[] content = [.. bom, .. Encoding.UTF8.GetBytes("héllo")];

        string text = await Create().ExtractTextAsync(content, "text/plain; charset=utf-8");

        text.Should().Be("héllo");
    }

    [Fact]
    public async Task ExtractTextAsync_Should_ReturnEmpty_ForEmptyContent()
    {
        string text = await Create().ExtractTextAsync([], "text/plain");

        text.Should().BeEmpty();
    }

    [Fact]
    public async Task ExtractTextAsync_Should_ReturnEmpty_ForUnsupportedType()
    {
        string text = await Create().ExtractTextAsync([1, 2, 3], "application/pdf");

        text.Should().BeEmpty();
    }

    [Fact]
    public async Task ExtractTextAsync_Should_DegradeGracefully_WhenBinaryMissing()
    {
        // Image type routes to the CLI; a missing binary must not throw, only yield empty text.
        var extractor = Create(executablePath: "lex-tesseract-does-not-exist");

        string text = await extractor.ExtractTextAsync([1, 2, 3, 4], "image/png");

        text.Should().BeEmpty();
    }
}
