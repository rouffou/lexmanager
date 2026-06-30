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
    private readonly IDocumentUnitOfWork _unitOfWork = Substitute.For<IDocumentUnitOfWork>();

    private UploadDocumentCommandHandler CreateHandler() => new(_repository, _caseApi, _storage, _unitOfWork);

    private static UploadDocumentCommand Command(byte[]? content = null) => new(
        Guid.NewGuid(), "piece.pdf", "application/pdf", DocumentCategory.ProcedureDocument, false,
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
}
