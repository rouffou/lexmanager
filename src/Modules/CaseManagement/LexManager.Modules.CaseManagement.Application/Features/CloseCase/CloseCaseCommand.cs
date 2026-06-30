using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.CaseManagement.Domain.Cases;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.CloseCase;

public sealed record CloseCaseCommand(Guid CaseId) : ICommand<Result>;

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

public sealed class CloseCaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/cases/{id:guid}/close", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(new CloseCaseCommand(id), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("CloseCase")
            .WithTags("Cases")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
