using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Contracts;
using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.GetDocumentById;

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
