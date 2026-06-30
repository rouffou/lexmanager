using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.OpenCarpaAccount;

/// <summary>Opens a third-party (CARPA / rubriqué) account for a case (SRD V11 §5).</summary>
public sealed record OpenCarpaAccountCommand(Guid CaseId, Guid ClientId, string Currency = Money.DefaultCurrency)
    : ICommand<Result<Guid>>;
