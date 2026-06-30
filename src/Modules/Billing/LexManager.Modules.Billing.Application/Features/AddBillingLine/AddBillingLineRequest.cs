namespace LexManager.Modules.Billing.Application.Features.AddBillingLine;

public sealed record AddBillingLineRequest(string Description, decimal Quantity, decimal UnitPrice);
