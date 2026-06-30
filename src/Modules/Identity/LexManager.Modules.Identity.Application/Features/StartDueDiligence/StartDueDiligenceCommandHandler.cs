using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Domain.Clients;
using LexManager.Modules.Identity.Domain.Compliance;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.StartDueDiligence;

public sealed class StartDueDiligenceCommandHandler(
    IClientRepository clientRepository,
    IClientDueDiligenceRepository repository,
    IIdentityUnitOfWork unitOfWork) : ICommandHandler<StartDueDiligenceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(StartDueDiligenceCommand request, CancellationToken cancellationToken = default)
    {
        Client? client = await clientRepository.GetByIdAsync(new ClientId(request.ClientId), cancellationToken);
        if (client is null)
        {
            return Result.Failure<Guid>(KycErrors.ClientNotFound);
        }

        if (await repository.ExistsForClientAsync(request.ClientId, cancellationToken))
        {
            return Result.Failure<Guid>(KycErrors.AlreadyExistsForClient);
        }

        var file = ClientDueDiligence.Start(request.ClientId, client.Type == ClientType.LegalPerson, request.RiskLevel);
        if (request.IsPoliticallyExposed)
        {
            file.FlagPoliticallyExposed(true);
        }

        repository.Add(file);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(file.Id.Value);
    }
}
