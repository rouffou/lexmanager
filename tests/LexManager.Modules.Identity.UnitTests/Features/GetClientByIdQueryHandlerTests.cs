using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Application.Features.GetClientById;
using LexManager.Modules.Identity.Contracts;
using LexManager.Modules.Identity.Domain.Clients;
using NSubstitute;

namespace LexManager.Modules.Identity.UnitTests.Features;

public class GetClientByIdQueryHandlerTests
{
    private readonly IClientReadRepository _readRepository = Substitute.For<IClientReadRepository>();

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenClientDoesNotExist()
    {
        _readRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ClientResponse?)null);

        var result = await new GetClientByIdQueryHandler(_readRepository)
            .Handle(new GetClientByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ClientErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnClient_WhenFound()
    {
        var id = Guid.NewGuid();
        var dto = new ClientResponse(id, "PhysicalPerson", "Jean Dupont", "jean@example.com",
            null, "Jean", "Dupont", "CNIE-1", null, null, null, DateTime.UtcNow);
        _readRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(dto);

        var result = await new GetClientByIdQueryHandler(_readRepository)
            .Handle(new GetClientByIdQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(dto);
    }
}
