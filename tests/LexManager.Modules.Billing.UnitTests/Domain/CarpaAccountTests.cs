using LexManager.Modules.Billing.Domain.Carpa;
using LexManager.Modules.Billing.Domain.Common;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Billing.UnitTests.Domain;

public class CarpaAccountTests
{
    private static CarpaAccount Open() => CarpaAccount.Open(Guid.NewGuid(), Guid.NewGuid());

    [Fact]
    public void Open_Should_StartEmpty_AndRaiseEvent()
    {
        CarpaAccount account = Open();

        account.Balance.Amount.Should().Be(0m);
        account.Currency.Should().Be("EUR");
        account.Transactions.Should().BeEmpty();
        account.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CarpaAccountOpenedDomainEvent>();
    }

    [Fact]
    public void Deposit_Should_IncreaseBalance_AndRecordMovement()
    {
        CarpaAccount account = Open();

        account.Deposit(Money.Of(1000m), "Provision client", "Client X");

        account.Balance.Amount.Should().Be(1000m);
        account.Transactions.Should().ContainSingle()
            .Which.Type.Should().Be(CarpaTransactionType.Deposit);
    }

    [Fact]
    public void Disburse_WithinBalance_Should_DecreaseBalance()
    {
        CarpaAccount account = Open();
        account.Deposit(Money.Of(1000m), "Provision");

        account.Disburse(Money.Of(400m), "Versement adverse", "Partie adverse");

        account.Balance.Amount.Should().Be(600m);
        account.Transactions.Should().HaveCount(2);
    }

    [Fact]
    public void Disburse_BeyondBalance_Should_Throw_InsufficientFunds()
    {
        CarpaAccount account = Open();
        account.Deposit(Money.Of(100m), "Provision");

        Action act = () => account.Disburse(Money.Of(150m), "Trop");

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(CarpaErrors.InsufficientFunds);
    }

    [Fact]
    public void Disburse_FromEmptyAccount_Should_Throw()
    {
        CarpaAccount account = Open();

        Action act = () => account.Disburse(Money.Of(1m), "Rien");

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(CarpaErrors.InsufficientFunds);
    }

    [Fact]
    public void Deposit_NonPositive_Should_Throw()
    {
        CarpaAccount account = Open();

        Action act = () => account.Deposit(Money.Of(0m), "Zero");

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(CarpaErrors.NonPositiveAmount);
    }

    [Fact]
    public void Balance_Should_NetDepositsAndDisbursements_InOrder()
    {
        CarpaAccount account = Open();

        account.Deposit(Money.Of(500m), "Provision 1");
        account.Deposit(Money.Of(300m), "Provision 2");
        account.Disburse(Money.Of(200m), "Frais");

        account.Balance.Amount.Should().Be(600m);
        account.Transactions.Should().HaveCount(3);
    }
}
