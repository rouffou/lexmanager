using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.CaseManagement.Domain.Cases;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.GetCaseById;

public sealed record GetCaseByIdQuery(Guid CaseId) : IQuery<Result<CaseResponse>>;

public sealed class GetCaseByIdQueryHandler(ICaseReadRepository readRepository)
    : IQueryHandler<GetCaseByIdQuery, Result<CaseResponse>>
{
    public async Task<Result<CaseResponse>> Handle(GetCaseByIdQuery request, CancellationToken cancellationToken = default)
    {
        CaseResponse? @case = await readRepository.GetByIdAsync(request.CaseId, cancellationToken);

        return @case is null
            ? Result.Failure<CaseResponse>(CaseErrors.NotFound)
            : Result.Success(@case);
    }
}

public sealed class GetCaseByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/cases/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<CaseResponse> result = await sender.Send(new GetCaseByIdQuery(id), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetCaseById")
            .WithTags("Cases")
            .Produces<CaseResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
