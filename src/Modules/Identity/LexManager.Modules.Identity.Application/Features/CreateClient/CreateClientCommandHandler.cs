using LexManager.Application.Abstractions.Persistence;
using LexManager.Modules.Identity.Domain.Clients;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.CreateClient;

public sealed class CreateClientCommandHandler(
    IClientRepository clientRepository,
    IConflictOfInterestChecker conflictChecker,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateClientCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateClientCommand request, CancellationToken cancellationToken = default)
    {
        Email email = Email.Create(request.Email);

        Client client = request.Type switch
        {
            ClientType.PhysicalPerson => Client.CreatePhysicalPerson(
                PersonName.Create(request.FirstName!, request.LastName!),
                request.NationalIdentityNumber!,
                email,
                request.Phone),

            _ => Client.CreateLegalPerson(
                Organization.Create(request.CompanyName!, request.RegistrationNumber!, request.LegalRepresentative ?? string.Empty),
                email,
                request.Phone)
        };

        // Mandatory global conflict-of-interest search before persisting (SRD Module 1).
        if (await conflictChecker.HasConflictAsync(client.IdentityKey, cancellationToken))
        {
            return Result.Failure<Guid>(ClientErrors.ConflictOfInterestDetected);
        }

        clientRepository.Add(client);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(client.Id.Value);
    }
}
