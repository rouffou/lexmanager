using LexManager.Modules.Billing.Domain.Common;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Billing.Domain.Carpa;

/// <summary>
/// A third-party (rubriqué/CARPA) account holding funds the firm manages on behalf of a client for
/// a case (SRD V11 §5). Aggregate root enforcing the cardinal rule of third-party accounting: the
/// balance can never go negative — client funds are never overdrawn.
/// </summary>
public sealed class CarpaAccount : AggregateRoot<CarpaAccountId>
{
    private readonly List<CarpaTransaction> _transactions = [];

    private CarpaAccount() { }

    private CarpaAccount(CarpaAccountId id, Guid caseId, Guid clientId, string currency) : base(id)
    {
        CaseId = caseId;
        ClientId = clientId;
        Currency = currency;
        OpenedOnUtc = DateTime.UtcNow;
    }

    public Guid CaseId { get; private set; }
    public Guid ClientId { get; private set; }
    public string Currency { get; private set; } = Money.DefaultCurrency;
    public DateTime OpenedOnUtc { get; private set; }

    public IReadOnlyList<CarpaTransaction> Transactions => _transactions.AsReadOnly();

    public Money Balance => Money.Of(_transactions.Sum(transaction => transaction.SignedAmount), Currency);

    public static CarpaAccount Open(Guid caseId, Guid clientId, string currency = Money.DefaultCurrency)
    {
        var account = new CarpaAccount(
            CarpaAccountId.New(), caseId, clientId,
            string.IsNullOrWhiteSpace(currency) ? Money.DefaultCurrency : currency.ToUpperInvariant());

        account.Raise(new CarpaAccountOpenedDomainEvent(account.Id.Value, caseId, clientId));
        return account;
    }

    public void Deposit(Money amount, string description, string? counterparty = null)
    {
        EnsurePositive(amount);
        Record(CarpaTransactionType.Deposit, amount, description, counterparty);
    }

    public void Disburse(Money amount, string description, string? counterparty = null)
    {
        EnsurePositive(amount);

        if (amount.Amount > Balance.Amount)
        {
            throw new BusinessRuleValidationException(CarpaErrors.InsufficientFunds);
        }

        Record(CarpaTransactionType.Disbursement, amount, description, counterparty);
    }

    private void Record(CarpaTransactionType type, Money amount, string description, string? counterparty)
    {
        _transactions.Add(new CarpaTransaction(
            type, amount, string.IsNullOrWhiteSpace(description) ? type.ToString() : description.Trim(), counterparty));

        Raise(new CarpaMovementRecordedDomainEvent(Id.Value, type, amount.Amount, Balance.Amount));
    }

    private static void EnsurePositive(Money amount)
    {
        if (amount.Amount <= 0)
        {
            throw new BusinessRuleValidationException(CarpaErrors.NonPositiveAmount);
        }
    }
}
