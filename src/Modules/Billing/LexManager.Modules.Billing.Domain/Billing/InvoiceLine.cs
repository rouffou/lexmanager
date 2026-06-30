using LexManager.Modules.Billing.Domain.Common;

namespace LexManager.Modules.Billing.Domain.Billing;

/// <summary>A single billed line: a quantity of something at a unit price.</summary>
public sealed class InvoiceLine
{
    private InvoiceLine() { }

    internal InvoiceLine(string description, decimal quantity, Money unitPrice)
    {
        Description = description;
        Quantity = quantity;
        UnitPriceAmount = unitPrice.Amount;
        Currency = unitPrice.Currency;
    }

    public string Description { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal UnitPriceAmount { get; private set; }
    public string Currency { get; private set; } = null!;

    public Money UnitPrice => Money.Of(UnitPriceAmount, Currency);
    public Money LineTotal => UnitPrice.Multiply(Quantity);
}
