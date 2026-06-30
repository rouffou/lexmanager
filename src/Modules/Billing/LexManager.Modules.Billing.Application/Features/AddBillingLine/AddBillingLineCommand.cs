using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.AddBillingLine;

public sealed record AddBillingLineCommand(Guid DocumentId, string Description, decimal Quantity, decimal UnitPrice)
    : ICommand<Result>;
