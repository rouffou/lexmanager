using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.RegisterPayment;

public sealed record RegisterPaymentCommand(Guid DocumentId) : ICommand<Result>;
