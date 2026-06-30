using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.ProcessOverdue;

/// <summary>
/// Flags issued documents whose due date has passed as overdue and fires a payment reminder
/// (SRD Module 5: relances automatiques). Invoked on demand here; a background worker will call
/// it on a schedule (cross-cutting concern).
/// </summary>
public sealed record ProcessOverdueCommand : ICommand<Result<int>>;
