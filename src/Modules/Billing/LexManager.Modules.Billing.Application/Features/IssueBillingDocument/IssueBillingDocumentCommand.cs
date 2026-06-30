using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.IssueBillingDocument;

public sealed record IssueBillingDocumentCommand(Guid DocumentId, DateTime DueDateUtc) : ICommand<Result<string>>;
