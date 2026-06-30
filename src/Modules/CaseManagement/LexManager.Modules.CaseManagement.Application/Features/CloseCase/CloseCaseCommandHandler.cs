using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Domain.Cases;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.CloseCase;

public sealed class CloseCaseCommandHandler(ICaseRepository caseRepository, ICaseUnitOfWork unitOfWork)
    : ICommandHandler<CloseCaseCommand, Result>
{
    public async Task<Result> Handle(CloseCaseCommand request, CancellationToken cancellationToken = default)
    {
        Case? @case = await caseRepository.GetByIdAsync(new CaseId(request.CaseId), cancellationToken);

        if (@case is null)
        {
            return Result.Failure(CaseErrors.NotFound);
        }

        if (@case.Status == CaseStatus.Closed)
        {
            return Result.Failure(CaseErrors.AlreadyClosed);
        }

        @case.Close();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
