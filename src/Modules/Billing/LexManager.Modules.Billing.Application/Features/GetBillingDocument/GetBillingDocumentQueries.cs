using LexManager.Application.Abstractions.Pagination;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.GetBillingDocument;

public sealed record GetBillingDocumentByIdQuery(Guid DocumentId) : IQuery<Result<BillingDocumentResponse>>;

public sealed class GetBillingDocumentByIdQueryHandler(IBillingReadRepository readRepository)
    : IQueryHandler<GetBillingDocumentByIdQuery, Result<BillingDocumentResponse>>
{
    public async Task<Result<BillingDocumentResponse>> Handle(GetBillingDocumentByIdQuery request, CancellationToken cancellationToken = default)
    {
        BillingDocumentResponse? document = await readRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        return document is null
            ? Result.Failure<BillingDocumentResponse>(BillingErrors.NotFound)
            : Result.Success(document);
    }
}

public sealed record GetBillingDocumentsByCaseQuery(Guid CaseId, int Page = 1, int PageSize = 25)
    : IQuery<Result<PagedList<BillingDocumentSummaryResponse>>>;

public sealed class GetBillingDocumentsByCaseQueryHandler(IBillingReadRepository readRepository)
    : IQueryHandler<GetBillingDocumentsByCaseQuery, Result<PagedList<BillingDocumentSummaryResponse>>>
{
    public async Task<Result<PagedList<BillingDocumentSummaryResponse>>> Handle(
        GetBillingDocumentsByCaseQuery request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new PaginationParameters(request.Page, request.PageSize);
        PagedList<BillingDocumentSummaryResponse> page =
            await readRepository.GetByCaseAsync(request.CaseId, parameters, cancellationToken);
        return Result.Success(page);
    }
}

public sealed class GetBillingDocumentByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/billing/documents/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<BillingDocumentResponse> result = await sender.Send(new GetBillingDocumentByIdQuery(id), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetBillingDocumentById")
            .WithTags("Billing")
            .Produces<BillingDocumentResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

public sealed class GetBillingDocumentsByCaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/billing/documents", async (
                Guid caseId,
                ISender sender,
                CancellationToken cancellationToken,
                int page = 1,
                int pageSize = 25) =>
            {
                Result<PagedList<BillingDocumentSummaryResponse>> result =
                    await sender.Send(new GetBillingDocumentsByCaseQuery(caseId, page, pageSize), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetBillingDocumentsByCase")
            .WithTags("Billing")
            .Produces<PagedList<BillingDocumentSummaryResponse>>();
    }
}
