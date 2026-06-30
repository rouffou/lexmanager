using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Billing.Domain.Common;

/// <summary>A monetary amount in a single currency. Immutable; arithmetic requires matching currencies.</summary>
public sealed class Money : ValueObject
{
    public const string DefaultCurrency = "EUR";

    private Money(decimal amount, string currency)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public static Money Zero(string currency = DefaultCurrency) => new(0m, currency);

    public static Money Of(decimal amount, string currency = DefaultCurrency)
    {
        if (amount < 0)
        {
            throw new BusinessRuleValidationException(BillingErrors.NegativeAmount);
        }

        return new Money(amount, string.IsNullOrWhiteSpace(currency) ? DefaultCurrency : currency.ToUpperInvariant());
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(decimal factor) => new(Amount * factor, Currency);

    public Money Percentage(decimal percent) => new(Amount * percent / 100m, Currency);

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new BusinessRuleValidationException(BillingErrors.CurrencyMismatch);
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
