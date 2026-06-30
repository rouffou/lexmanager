namespace LexManager.Modules.Billing.Application.Features.RecordCarpaDeposit;

public sealed record RecordCarpaDepositRequest(decimal Amount, string Description, string? Counterparty);
