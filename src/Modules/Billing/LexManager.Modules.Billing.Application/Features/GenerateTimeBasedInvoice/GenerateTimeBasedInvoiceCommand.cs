using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.GenerateTimeBasedInvoice;

/// <summary>
/// Builds and issues a time-based invoice for a case by pulling billable time from the Calendar
/// module's <c>ITimeTrackingApi</c> and applying an hourly rate (SRD Module 5: au temps passé).
/// </summary>
public sealed record GenerateTimeBasedInvoiceCommand(
    Guid CaseId,
    Guid ClientId,
    decimal HourlyRate,
    DateTime DueDateUtc,
    decimal TaxRatePercent = 21m) : ICommand<Result<Guid>>; // Belgian VAT (SRD §5)
