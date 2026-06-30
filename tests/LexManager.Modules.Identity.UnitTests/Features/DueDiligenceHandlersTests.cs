using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Application.Features.DecideDueDiligence;
using LexManager.Modules.Identity.Application.Features.RecordVerificationCheck;
using LexManager.Modules.Identity.Application.Features.StartDueDiligence;
using LexManager.Modules.Identity.Domain.Clients;
using LexManager.Modules.Identity.Domain.Compliance;
using NSubstitute;

namespace LexManager.Modules.Identity.UnitTests.Features;

public class DueDiligenceHandlersTests
{
    private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
    private readonly IClientDueDiligenceRepository _repository = Substitute.For<IClientDueDiligenceRepository>();
    private readonly IIdentityUnitOfWork _unitOfWork = Substitute.For<IIdentityUnitOfWork>();

    private static Client LegalClient() => Client.CreateLegalPerson(
        Organization.Create("SNE Avocats", "12345678901234", "Maître Martin"), Email.Create("firm@example.com"));

    [Fact]
    public async Task Start_Should_Fail_WhenClientMissing()
    {
        _clientRepository.GetByIdAsync(Arg.Any<ClientId>(), Arg.Any<CancellationToken>()).Returns((Client?)null);
        var handler = new StartDueDiligenceCommandHandler(_clientRepository, _repository, _unitOfWork);

        var result = await handler.Handle(new StartDueDiligenceCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(KycErrors.ClientNotFound);
    }

    [Fact]
    public async Task Start_Should_Fail_WhenFileAlreadyExists()
    {
        _clientRepository.GetByIdAsync(Arg.Any<ClientId>(), Arg.Any<CancellationToken>()).Returns(LegalClient());
        _repository.ExistsForClientAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = new StartDueDiligenceCommandHandler(_clientRepository, _repository, _unitOfWork);

        var result = await handler.Handle(new StartDueDiligenceCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(KycErrors.AlreadyExistsForClient);
    }

    [Fact]
    public async Task Start_Should_Persist_AndInferLegalEntityFromClient()
    {
        _clientRepository.GetByIdAsync(Arg.Any<ClientId>(), Arg.Any<CancellationToken>()).Returns(LegalClient());
        _repository.ExistsForClientAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        ClientDueDiligence? added = null;
        _repository.When(r => r.Add(Arg.Any<ClientDueDiligence>())).Do(call => added = call.Arg<ClientDueDiligence>());

        var handler = new StartDueDiligenceCommandHandler(_clientRepository, _repository, _unitOfWork);
        var result = await handler.Handle(new StartDueDiligenceCommand(Guid.NewGuid(), RiskLevel.High), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        added.Should().NotBeNull();
        added!.IsLegalEntity.Should().BeTrue();
        added.RequiredChecks.Should().Contain(VerificationKind.SanctionsScreening); // high risk
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RecordCheck_Should_Fail_WhenFileMissing()
    {
        _repository.GetByIdAsync(Arg.Any<DueDiligenceId>(), Arg.Any<CancellationToken>()).Returns((ClientDueDiligence?)null);
        var handler = new RecordVerificationCheckCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(
            new RecordVerificationCheckCommand(Guid.NewGuid(), VerificationKind.IdentityDocument, "PASS-1", true, null),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(KycErrors.NotFound);
    }

    [Fact]
    public async Task RecordCheck_Should_Persist_WhenInProgress()
    {
        ClientDueDiligence file = ClientDueDiligence.Start(Guid.NewGuid(), isLegalEntity: false, RiskLevel.Standard);
        _repository.GetByIdAsync(Arg.Any<DueDiligenceId>(), Arg.Any<CancellationToken>()).Returns(file);
        var handler = new RecordVerificationCheckCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(
            new RecordVerificationCheckCommand(file.Id.Value, VerificationKind.IdentityDocument, "PASS-1", true, "ok"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        file.Checks.Should().ContainSingle();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Decide_Approve_Should_Fail_WhenScoreIncomplete()
    {
        ClientDueDiligence file = ClientDueDiligence.Start(Guid.NewGuid(), isLegalEntity: false, RiskLevel.Standard);
        file.RecordCheck(VerificationKind.IdentityDocument, "PASS-1", cleared: true); // only 50%
        _repository.GetByIdAsync(Arg.Any<DueDiligenceId>(), Arg.Any<CancellationToken>()).Returns(file);
        var handler = new DecideDueDiligenceCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(new DecideDueDiligenceCommand(file.Id.Value, Approve: true, Reason: null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(KycErrors.IncompleteDueDiligence);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Decide_Approve_Should_Succeed_WhenScoreComplete()
    {
        ClientDueDiligence file = ClientDueDiligence.Start(Guid.NewGuid(), isLegalEntity: false, RiskLevel.Standard);
        file.RecordCheck(VerificationKind.IdentityDocument, "PASS-1", cleared: true);
        file.RecordCheck(VerificationKind.AddressProof, "BILL-1", cleared: true);
        _repository.GetByIdAsync(Arg.Any<DueDiligenceId>(), Arg.Any<CancellationToken>()).Returns(file);
        var handler = new DecideDueDiligenceCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(new DecideDueDiligenceCommand(file.Id.Value, Approve: true, Reason: null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        file.Status.Should().Be(DueDiligenceStatus.Approved);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Decide_Reject_Should_Succeed_WithReason()
    {
        ClientDueDiligence file = ClientDueDiligence.Start(Guid.NewGuid(), isLegalEntity: false, RiskLevel.Standard);
        _repository.GetByIdAsync(Arg.Any<DueDiligenceId>(), Arg.Any<CancellationToken>()).Returns(file);
        var handler = new DecideDueDiligenceCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(
            new DecideDueDiligenceCommand(file.Id.Value, Approve: false, Reason: "Soupçon de blanchiment"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        file.Status.Should().Be(DueDiligenceStatus.Rejected);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
