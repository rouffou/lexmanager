using LexManager.Modules.Documents.Infrastructure.Persistence;

namespace LexManager.Modules.Documents.UnitTests.Infrastructure;

public class SearchHighlighterTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BuildSnippet_Should_ReturnNull_WhenNoText(string? text)
    {
        SearchHighlighter.BuildSnippet(text, "contrat").Should().BeNull();
    }

    [Fact]
    public void BuildSnippet_Should_CenterOnFirstMatch()
    {
        string body = new string('a', 200) + " le contrat de bail signé " + new string('b', 200);

        string? snippet = SearchHighlighter.BuildSnippet(body, "contrat");

        snippet.Should().NotBeNull();
        snippet!.Should().Contain("contrat");
        snippet.Should().StartWith("…");
    }

    [Fact]
    public void BuildSnippet_Should_CollapseWhitespace()
    {
        string? snippet = SearchHighlighter.BuildSnippet("contrat\n\n  de   bail", "contrat");

        snippet.Should().Be("contrat de bail");
    }

    [Fact]
    public void BuildSnippet_Should_ReturnLeadingExcerpt_WhenTermNotInBody()
    {
        // Match was on the file name, not the OCR body.
        string? snippet = SearchHighlighter.BuildSnippet("procès-verbal de constat", "assignation");

        snippet.Should().Be("procès-verbal de constat");
    }

    [Fact]
    public void BuildSnippet_Should_Truncate_LongLeadingExcerpt()
    {
        string body = new string('x', 400);

        string? snippet = SearchHighlighter.BuildSnippet(body, "y");

        snippet!.Length.Should().BeLessThan(200);
        snippet.Should().EndWith("…");
    }
}
