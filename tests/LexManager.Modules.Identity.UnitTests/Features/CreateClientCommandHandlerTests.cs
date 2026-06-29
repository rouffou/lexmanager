using LexManager.Application.Abstractions.Persistence;
using LexManager.Modules.Identity.Application.Features.CreateClient;
using LexManager.Modules.Identity.Domain.Clients;
using NSubstitute;

namespace LexManager.Modules.Identity.UnitTests.Features;

public class CreateClientCommandHandlerTests
{
    private readonly IClientRepository _repository = Substitute.For<IClientRepository>();
    private readonly IConflictOfInterestChecker _conflictChecker = Substitute.For<IConflictOfInterestChecker>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private CreateClientCommandHandler CreateHandler() => new(_repository, _conflictChecker, _unitOfWork);

    private static CreateClientCommand PhysicalCommand => new(
        ClientType.PhysicalPerson,
        "jean.dupont@example.com",
        Phone: null,
        FirstName: "Jean",
        LastName: "Dupont",
        NationalIdentityNumber: "CNIE-12345",
        CompanyName: null,
        RegistrationNumber: null,
        LegalRepresentative: null);

    [Fact]
    public async Task Handle_Should_ReturnError_WhenConflictOfInterestIsDetected()
    {
        _conflictChecker.HasConflictAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        var result = await CreateHandler().Handle(PhysicalCommand, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ClientErrors.ConflictOfInterestDetected);
        _repository.DidNotReceive().Add(Arg.Any<Client>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_PersistClientAndReturnId_WhenNoConflict()
    {
        _conflictChecker.HasConflictAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await CreateHandler().Handle(PhysicalCommand, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _repository.Received(1).Add(Arg.Is<Client>(c => c.DisplayName == "Jean Dupont"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_CheckConflict_UsingIdentityKey()
    {
        _conflictChecker.HasConflictAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        await CreateHandler().Handle(PhysicalCommand, CancellationToken.None);

        await _conflictChecker.Received(1).HasConflictAsync("CNIE-12345", Arg.Any<CancellationToken>());
    }
}
