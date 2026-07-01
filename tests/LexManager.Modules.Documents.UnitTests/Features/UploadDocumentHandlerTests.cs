using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Application.Features.UploadDocument;
using LexManager.Modules.Documents.Domain.Documents;
using NSubstitute;

namespace LexManager.Modules.Documents.UnitTests.Features;

public class UploadDocumentHandlerTests
{
    private readonly IDocumentRepository _repository = Substitute.For<IDocumentRepository>();
    private readonly ICaseApi _caseApi = Substitute.For<ICaseApi>();
    private readonly IDocumentStorage _storage = Substitute.For<IDocumentStorage>();
    private readonly IOcrTextExtractor _ocr = Substitute.For<IOcrTextExtractor>();
    private readonly IDocumentUnitOfWork _unitOfWork = Substitute.For<IDocumentUnitOfWork>();

    private UploadDocumentCommandHandler CreateHandler() => new(_repository, _caseApi, _storage, _ocr, _unitOfWork);

    private static UploadDocumentCommand Command(byte[]? content = null, string contentType = "application/pdf") => new(
        Guid.NewGuid(), "piece.pdf", contentType, DocumentCategory.ProcedureDocument, false,
        content ?? [1, 2, 3, 4]);

    [Fact]
    public async Task Handle_Should_Fail_WhenCaseDoesNotExist()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DocumentErrors.CaseNotFound);
        _repository.DidNotReceive().Add(Arg.Any<Document>());
    }

    [Fact]
    public async Task Handle_Should_Fail_WhenContentEmpty()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        var result = await CreateHandler().Handle(Command([]), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DocumentErrors.EmptyContent);
    }

    [Fact]
    public async Task Handle_Should_StoreAndPersist_WhenValid()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _storage.SaveAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(new StoredFile("2026/06/key", 4, "checksum"));

        var result = await CreateHandler().Handle(Command(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _storage.Received(1).SaveAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
        _repository.Received(1).Add(Arg.Is<Document>(d => d.Category == DocumentCategory.ProcedureDocument));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_IndexExtractedText_WhenOcrCanExtract()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _storage.SaveAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(new StoredFile("2026/06/key", 4, "checksum"));
        _ocr.CanExtract("text/plain").Returns(true);
        _ocr.ExtractTextAsync(Arg.Any<byte[]>(), "text/plain", Arg.Any<CancellationToken>())
            .Returns("conclusions en demande");

        Document? saved = null;
        _repository.When(r => r.Add(Arg.Any<Document>())).Do(call => saved = call.Arg<Document>());

        var result = await CreateHandler().Handle(Command(contentType: "text/plain"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        saved.Should().NotBeNull();
        saved!.IsIndexed.Should().BeTrue();
        saved.ExtractedText.Should().Be("conclusions en demande");
    }

    [Fact]
    public async Task Handle_Should_SkipOcr_WhenContentTypeUnsupported()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _storage.SaveAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(new StoredFile("2026/06/key", 4, "checksum"));
        _ocr.CanExtract(Arg.Any<string>()).Returns(false);

        await CreateHandler().Handle(Command(contentType: "application/zip"), CancellationToken.None);

        await _ocr.DidNotReceive().ExtractTextAsync(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
