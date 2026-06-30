namespace LexManager.Modules.Billing.Contracts;

public sealed record CarpaAccountResponse(
    Guid Id,
    Guid CaseId,
    Guid ClientId,
    string Currency,
    decimal Balance,
    IReadOnlyList<CarpaTransactionResponse> Transactions,
    DateTime OpenedOnUtc);
