using LexManager.Application.Abstractions.Messaging;

namespace LexManager.Modules.Documents.Contracts;

/// <summary>Documents module's public cross-module contract.</summary>
public interface IDocumentApi : IModuleApi
{
    Task<int> CountByCaseAsync(Guid caseId, CancellationToken cancellationToken = default);
}
