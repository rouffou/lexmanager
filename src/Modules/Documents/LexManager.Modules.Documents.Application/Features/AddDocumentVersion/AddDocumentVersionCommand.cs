using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.AddDocumentVersion;

public sealed record AddDocumentVersionCommand(Guid DocumentId, byte[] Content) : ICommand<Result<int>>;
