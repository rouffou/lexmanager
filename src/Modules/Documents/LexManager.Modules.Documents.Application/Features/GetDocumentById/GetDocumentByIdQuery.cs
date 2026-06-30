using LexManager.Modules.Documents.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.GetDocumentById;

public sealed record GetDocumentByIdQuery(Guid DocumentId) : IQuery<Result<DocumentResponse>>;
