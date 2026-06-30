using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Contracts;

namespace LexManager.Modules.Documents.Infrastructure.PublicApi;

internal sealed class DocumentApi(IDocumentReadRepository readRepository) : IDocumentApi
{
    public Task<int> CountByCaseAsync(Guid caseId, CancellationToken cancellationToken = default) =>
        readRepository.CountByCaseAsync(caseId, cancellationToken);
}
