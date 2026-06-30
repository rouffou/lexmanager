using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Application.Features.CloseCase;
using LexManager.Modules.CaseManagement.Application.Features.CreateCase;
using LexManager.Modules.CaseManagement.Domain.Cases;
using LexManager.Modules.Identity.Contracts;
using NSubstitute;

namespace LexManager.Modules.CaseManagement.UnitTests.Features;

public class CaseHandlersTests
{
    private readonly ICaseRepository _repository = Substitute.For<ICaseRepository>();
    private readonly IClientApi _clientApi = Substitute.For<IClientApi>();
    private readonly ICaseUnitOfWork _unitOfWork = Substitute.For<ICaseUnitOfWork>();

    [Fact]
    public async Task CreateCase_Should_Fail_WhenClientDoesNotExist()
    {
        _clientApi.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = new CreateCaseCommandHandler(_repository, _clientApi, _unitOfWork);

        var result = await handler.Handle(new CreateCaseCommand("Litige", Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CaseErrors.ClientNotFound);
        _repository.DidNotReceive().Add(Arg.Any<Case>());
    }

    [Fact]
    public async Task CreateCase_Should_Persist_WhenClientExists()
    {
        _clientApi.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = new CreateCaseCommandHandler(_repository, _clientApi, _unitOfWork);

        var result = await handler.Handle(
            new CreateCaseCommand("Litige", Guid.NewGuid(), "Tribunal judiciaire", "RG-2026-001"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repository.Received(1).Add(Arg.Any<Case>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CloseCase_Should_ReturnNotFound_WhenMissing()
    {
        _repository.GetByIdAsync(Arg.Any<CaseId>(), Arg.Any<CancellationToken>()).Returns((Case?)null);
        var handler = new CloseCaseCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(new CloseCaseCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CaseErrors.NotFound);
    }

    [Fact]
    public async Task CloseCase_Should_Succeed_WhenOpen()
    {
        Case @case = Case.Open("Dossier", Guid.NewGuid());
        _repository.GetByIdAsync(Arg.Any<CaseId>(), Arg.Any<CancellationToken>()).Returns(@case);
        var handler = new CloseCaseCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(new CloseCaseCommand(@case.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @case.Status.Should().Be(CaseStatus.Closed);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
