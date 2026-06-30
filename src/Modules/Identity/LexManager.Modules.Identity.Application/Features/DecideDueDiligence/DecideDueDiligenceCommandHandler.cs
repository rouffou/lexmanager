using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Domain.Compliance;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.DecideDueDiligence;

public sealed class DecideDueDiligenceCommandHandler(
    IClientDueDiligenceRepository repository,
    IIdentityUnitOfWork unitOfWork) : ICommandHandler<DecideDueDiligenceCommand, Result>
{
    public async Task<Result> Handle(DecideDueDiligenceCommand request, CancellationToken cancellationToken = default)
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

        if (request.Approve)
        {
            // Gate the mandate: cannot accept until every required vigilance check is cleared.
            if (!file.CanApprove)
            {
                return Result.Failure(KycErrors.IncompleteDueDiligence);
            }

            file.Approve();
        }
        else
        {
            file.Reject(request.Reason ?? string.Empty);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
