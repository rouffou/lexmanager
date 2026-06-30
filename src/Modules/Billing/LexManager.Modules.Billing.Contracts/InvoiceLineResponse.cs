namespace LexManager.Modules.Billing.Contracts;

public sealed record InvoiceLineResponse(string Description, decimal Quantity, decimal UnitPrice, decimal LineTotal);
