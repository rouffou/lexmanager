using LexManager.Modules.Billing.Domain.Common;

namespace LexManager.Modules.Billing.Domain.Carpa;

/// <summary>
/// One immutable movement on a third-party account. Full traceability is mandatory for the Bars
/// (AVOCATS.BE / OVB), so movements are append-only with a counterparty and timestamp (SRD V11 §5).
/// </summary>
public sealed class CarpaTransaction
{
    private CarpaTransaction() { }

    internal CarpaTransaction(CarpaTransactionType type, Money amount, string description, string? counterparty)
    {
        Type = type;
        AmountValue = amount.Amount;
        Currency = amount.Currency;
        Description = description;
        Counterparty = counterparty;
        OccurredOnUtc = DateTime.UtcNow;
    }

    public CarpaTransactionType Type { get; private set; }
    public decimal AmountValue { get; private set; }
    public string Currency { get; private set; } = Money.DefaultCurrency;
    public string Description { get; private set; } = null!;
    public string? Counterparty { get; private set; }
    public DateTime OccurredOnUtc { get; private set; }

    public Money Amount => Money.Of(AmountValue, Currency);

    /// <summary>Deposits add to the balance, disbursements subtract.</summary>
    public decimal SignedAmount => Type == CarpaTransactionType.Deposit ? AmountValue : -AmountValue;
}
