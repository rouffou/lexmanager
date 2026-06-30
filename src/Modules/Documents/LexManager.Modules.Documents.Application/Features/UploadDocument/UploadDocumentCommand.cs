using LexManager.Modules.Documents.Domain.Documents;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.UploadDocument;

public sealed record UploadDocumentCommand(
    Guid CaseId,
    string FileName,
    string ContentType,
    DocumentCategory Category,
    bool IsConfidential,
    byte[] Content) : ICommand<Result<Guid>>;
