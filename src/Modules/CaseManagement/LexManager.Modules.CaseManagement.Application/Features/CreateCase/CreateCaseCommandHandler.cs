using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Domain.Cases;
using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.CreateCase;

public sealed class CreateCaseCommandHandler(
    ICaseRepository caseRepository,
    IClientApi clientApi,
    ICaseUnitOfWork unitOfWork) : ICommandHandler<CreateCaseCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCaseCommand request, CancellationToken cancellationToken = default)
    {
        // Cross-module check through Identity's public contract only (SRD §3.2).
        if (!await clientApi.ClientExistsAsync(request.ClientId, cancellationToken))
        {
            return Result.Failure<Guid>(CaseErrors.ClientNotFound);
        }

        Jurisdiction? jurisdiction = request.CourtName is not null && request.GeneralRegisterNumber is not null
            ? Jurisdiction.Create(request.CourtName, request.GeneralRegisterNumber, request.Judge)
            : null;

        Case @case = Case.Open(request.Title, request.ClientId, jurisdiction);

        caseRepository.Add(@case);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(@case.Id.Value);
    }
}
