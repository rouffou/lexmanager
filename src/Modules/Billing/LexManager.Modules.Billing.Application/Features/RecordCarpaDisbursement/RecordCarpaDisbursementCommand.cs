using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.RecordCarpaDisbursement;

public sealed record RecordCarpaDisbursementCommand(Guid AccountId, decimal Amount, string Description, string? Counterparty)
    : ICommand<Result>;
