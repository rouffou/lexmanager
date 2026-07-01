using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Application.Features.AddDocumentVersion;
using LexManager.Modules.Documents.Domain.Documents;
using NSubstitute;

namespace LexManager.Modules.Documents.UnitTests.Features;

public class AddDocumentVersionHandlerTests
{
    private readonly IDocumentRepository _repository = Substitute.For<IDocumentRepository>();
    private readonly IDocumentStorage _storage = Substitute.For<IDocumentStorage>();
    private readonly IOcrTextExtractor _ocr = Substitute.For<IOcrTextExtractor>();
    private readonly IDocumentUnitOfWork _unitOfWork = Substitute.For<IDocumentUnitOfWork>();

    private AddDocumentVersionCommandHandler CreateHandler() => new(_repository, _storage, _ocr, _unitOfWork);

    private static Document TextDocument() => Document.Create(
        Guid.NewGuid(), "conclusions.txt", "text/plain", DocumentCategory.Conclusions, "k", 1, "c");

    [Fact]
    public async Task Handle_Should_Fail_WhenContentEmpty()
    {
        var result = await CreateHandler().Handle(new AddDocumentVersionCommand(Guid.NewGuid(), []), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DocumentErrors.EmptyContent);
    }

    [Fact]
    public async Task Handle_Should_Fail_WhenDocumentNotFound()
    {
        _repository.GetByIdAsync(Arg.Any<DocumentId>(), Arg.Any<CancellationToken>()).Returns((Document?)null);

        var result = await CreateHandler().Handle(new AddDocumentVersionCommand(Guid.NewGuid(), [1, 2]), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DocumentErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_AddVersion_AndReindex_WhenOcrCanExtract()
    {
        Document document = TextDocument();
        _repository.GetByIdAsync(Arg.Any<DocumentId>(), Arg.Any<CancellationToken>()).Returns(document);
        _storage.SaveAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(new StoredFile("2026/07/key", 6, "sum"));
        _ocr.CanExtract("text/plain").Returns(true);
        _ocr.ExtractTextAsync(Arg.Any<byte[]>(), "text/plain", Arg.Any<CancellationToken>()).Returns("version deux");

        var result = await CreateHandler().Handle(new AddDocumentVersionCommand(document.Id.Value, [9, 9]), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(2);
        document.CurrentVersionNumber.Should().Be(2);
        document.ExtractedText.Should().Be("version deux");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
