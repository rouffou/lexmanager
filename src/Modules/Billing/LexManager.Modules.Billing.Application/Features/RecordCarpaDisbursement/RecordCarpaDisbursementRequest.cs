namespace LexManager.Modules.Billing.Application.Features.RecordCarpaDisbursement;

public sealed record RecordCarpaDisbursementRequest(decimal Amount, string Description, string? Counterparty);
