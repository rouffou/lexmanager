using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Application.Features.SearchDocuments;
using LexManager.Modules.Documents.Contracts;
using NSubstitute;

namespace LexManager.Modules.Documents.UnitTests.Features;

public class SearchDocumentsHandlerTests
{
    private readonly IDocumentReadRepository _readRepository = Substitute.For<IDocumentReadRepository>();

    [Fact]
    public async Task Handle_Should_ReturnPage_FromReadRepository()
    {
        var caseId = Guid.NewGuid();
        var hit = new DocumentSearchResultResponse(
            Guid.NewGuid(), caseId, "assignation.pdf", "ProcedureDocument", "… assignation à comparaître …", DateTime.UtcNow);
        var page = new PagedList<DocumentSearchResultResponse>([hit], 1, 25, 1);

        _readRepository
            .SearchAsync("assignation", caseId, Arg.Any<PaginationParameters>(), Arg.Any<CancellationToken>())
            .Returns(page);

        var handler = new SearchDocumentsQueryHandler(_readRepository);
        var result = await handler.Handle(new SearchDocumentsQuery("assignation", caseId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(1);
        result.Value.Items.Should().ContainSingle().Which.FileName.Should().Be("assignation.pdf");
    }

    [Fact]
    public async Task Handle_Should_TrimTerm_BeforeSearching()
    {
        _readRepository
            .SearchAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<PaginationParameters>(), Arg.Any<CancellationToken>())
            .Returns(new PagedList<DocumentSearchResultResponse>([], 1, 25, 0));

        var handler = new SearchDocumentsQueryHandler(_readRepository);
        await handler.Handle(new SearchDocumentsQuery("  bail  "), CancellationToken.None);

        await _readRepository.Received(1)
            .SearchAsync("bail", null, Arg.Any<PaginationParameters>(), Arg.Any<CancellationToken>());
    }
}
