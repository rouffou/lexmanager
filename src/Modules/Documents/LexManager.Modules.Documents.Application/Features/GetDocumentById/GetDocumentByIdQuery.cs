using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Contracts;
using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Documents.Application.Features.GetDocumentById;

public sealed record GetDocumentByIdQuery(Guid DocumentId) : IQuery<Result<DocumentResponse>>;

public sealed class GetDocumentByIdQueryHandler(IDocumentReadRepository readRepository)
    : IQueryHandler<GetDocumentByIdQuery, Result<DocumentResponse>>
{
    public async Task<Result<DocumentResponse>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken = default)
    {
        DocumentResponse? document = await readRepository.GetByIdAsync(request.DocumentId, cancellationToken);

        return document is null
            ? Result.Failure<DocumentResponse>(DocumentErrors.NotFound)
            : Result.Success(document);
    }
}

public sealed class GetDocumentByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/documents/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<DocumentResponse> result = await sender.Send(new GetDocumentByIdQuery(id), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetDocumentById")
            .WithTags("Documents")
            .Produces<DocumentResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
