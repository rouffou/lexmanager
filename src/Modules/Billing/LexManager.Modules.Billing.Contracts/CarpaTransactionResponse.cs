namespace LexManager.Modules.Billing.Contracts;

public sealed record CarpaTransactionResponse(
    string Type,
    decimal Amount,
    string Description,
    string? Counterparty,
    DateTime OccurredOnUtc);
