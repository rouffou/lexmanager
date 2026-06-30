using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.RecordCarpaDeposit;

public sealed record RecordCarpaDepositCommand(Guid AccountId, decimal Amount, string Description, string? Counterparty)
    : ICommand<Result>;
