using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;

namespace LexManager.Modules.Billing.UnitTests.Domain;

public class VatRegimeTests
{
    private static BillingDocument Draft(VatRegime regime) => BillingDocument.CreateDraft(
        Guid.NewGuid(), Guid.NewGuid(), BillingDocumentKind.Invoice, BillingMode.Flat,
        taxRatePercent: 21m, currency: Money.DefaultCurrency, vatRegime: regime);

    [Fact]
    public void Standard_Should_Keep21Percent_AndNoMention()
    {
        BillingDocument document = Draft(VatRegime.Standard);
        document.AddLine("Honoraires", 1m, Money.Of(1000m));

        document.TaxRatePercent.Should().Be(21m);
        document.LegalMention.Should().BeNull();
        document.TaxAmount.Amount.Should().Be(210m);
        document.Total.Amount.Should().Be(1210m);
    }

    [Fact]
    public void ProDeo_Should_ExemptVat_AndCarryMention()
    {
        BillingDocument document = Draft(VatRegime.ProDeo);
        document.AddLine("Honoraires", 1m, Money.Of(1000m));

        document.TaxRatePercent.Should().Be(0m);
        document.VatRegime.Should().Be(VatRegime.ProDeo);
        document.LegalMention.Should().Contain("pro deo");
        document.TaxAmount.Amount.Should().Be(0m);
        document.Total.Amount.Should().Be(1000m);
    }

    [Fact]
    public void IntraCommunity_Should_ReverseCharge_WithMention()
    {
        BillingDocument document = Draft(VatRegime.IntraCommunityReverseCharge);

        document.TaxRatePercent.Should().Be(0m);
        document.LegalMention.Should().Contain("Autoliquidation");
    }
}
