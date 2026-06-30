using LexManager.Modules.Documents.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.DownloadDocument;

public sealed record DownloadDocumentQuery(Guid DocumentId, int? Version) : IQuery<Result<DocumentDownload>>;
