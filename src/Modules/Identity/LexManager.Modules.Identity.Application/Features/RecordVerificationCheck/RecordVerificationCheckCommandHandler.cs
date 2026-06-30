using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Domain.Compliance;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.RecordVerificationCheck;

public sealed class RecordVerificationCheckCommandHandler(
    IClientDueDiligenceRepository repository,
    IIdentityUnitOfWork unitOfWork) : ICommandHandler<RecordVerificationCheckCommand, Result>
{
    public async Task<Result> Handle(RecordVerificationCheckCommand request, CancellationToken cancellationToken = default)
    {
        ClientDueDiligence? file = await repository.GetByIdAsync(new DueDiligenceId(request.DueDiligenceId), cancellationToken);
        if (file is null)
        {
            return Result.Failure(KycErrors.NotFound);
        }

        if (file.Status != DueDiligenceStatus.InProgress)
        {
            return Result.Failure(KycErrors.AlreadyDecided);
        }

        file.RecordCheck(request.Kind, request.Reference, request.Cleared, request.Notes);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
