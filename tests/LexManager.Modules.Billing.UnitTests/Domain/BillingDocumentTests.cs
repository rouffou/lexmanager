using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Billing.UnitTests.Domain;

public class BillingDocumentTests
{
    private static BillingDocument Draft() => BillingDocument.CreateDraft(
        Guid.NewGuid(), Guid.NewGuid(), BillingDocumentKind.Invoice, BillingMode.Flat, taxRatePercent: 20m);

    [Fact]
    public void Money_Of_Should_Throw_OnNegative()
    {
        Action act = () => Money.Of(-1m);
        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(BillingErrors.NegativeAmount);
    }

    [Fact]
    public void Totals_Should_ComputeSubtotalTaxAndTotal()
    {
        BillingDocument document = Draft();
        document.AddLine("Consultation", 2m, Money.Of(100m));

        document.Subtotal.Amount.Should().Be(200m);
        document.TaxAmount.Amount.Should().Be(40m);
        document.Total.Amount.Should().Be(240m);
    }

    [Fact]
    public void Issue_Should_Throw_WhenNoLines()
    {
        BillingDocument document = Draft();

        Action act = () => document.Issue("FAC-2026-000001", DateTime.UtcNow.AddDays(30));

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(BillingErrors.CannotIssueEmpty);
    }

    [Fact]
    public void Issue_Should_SetIssued_AndRaiseEvent()
    {
        BillingDocument document = Draft();
        document.AddLine("Forfait", 1m, Money.Of(1500m));

        document.Issue("FAC-2026-000001", DateTime.UtcNow.AddDays(30));

        document.Status.Should().Be(BillingStatus.Issued);
        document.Number.Should().Be("FAC-2026-000001");
        document.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BillingDocumentIssuedDomainEvent>();
    }

    [Fact]
    public void AddLine_AfterIssue_Should_Throw()
    {
        BillingDocument document = Draft();
        document.AddLine("Forfait", 1m, Money.Of(1500m));
        document.Issue("FAC-2026-000001", DateTime.UtcNow.AddDays(30));

        Action act = () => document.AddLine("Extra", 1m, Money.Of(100m));

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(BillingErrors.NotEditable);
    }

    [Fact]
    public void RegisterPayment_FromDraft_Should_Throw()
    {
        BillingDocument document = Draft();

        Action act = () => document.RegisterPayment();

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(BillingErrors.NotIssued);
    }

    [Fact]
    public void RegisterPayment_Should_SetPaid_AndRaiseEvent()
    {
        BillingDocument document = Draft();
        document.AddLine("Forfait", 1m, Money.Of(1000m));
        document.Issue("FAC-2026-000001", DateTime.UtcNow.AddDays(30));
        document.ClearDomainEvents();

        document.RegisterPayment();

        document.Status.Should().Be(BillingStatus.Paid);
        document.PaidOnUtc.Should().NotBeNull();
        document.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<PaymentRegisteredDomainEvent>();
    }

    [Fact]
    public void MarkOverdueIfDue_Should_FlagPastDueIssuedDocuments()
    {
        BillingDocument document = Draft();
        document.AddLine("Forfait", 1m, Money.Of(1000m));
        document.Issue("FAC-2026-000001", DateTime.UtcNow.AddDays(-1));

        bool changed = document.MarkOverdueIfDue(DateTime.UtcNow);

        changed.Should().BeTrue();
        document.Status.Should().Be(BillingStatus.Overdue);
    }
}
