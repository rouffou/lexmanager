using LexManager.Modules.Billing.Domain.Common;
using LexManager.Modules.Billing.Domain.Interest;

namespace LexManager.Modules.Billing.UnitTests.Domain;

public class BelgianLegalInterestTests
{
    private static readonly Money Principal = Money.Of(10_000m);

    [Fact]
    public void Simple_OverOneYear_Should_EqualPrincipalTimesRate()
    {
        LegalInterestResult result = BelgianLegalInterestCalculator.Compute(
            Principal, new DateOnly(2023, 1, 1), new DateOnly(2024, 1, 1), annualRatePercent: 8m);

        result.Days.Should().Be(365);
        result.Interest.Amount.Should().Be(800m);
        result.Total.Amount.Should().Be(10_800m);
    }

    [Fact]
    public void Capitalized_OverOneYear_Should_MatchSimple()
    {
        LegalInterestResult capitalized = BelgianLegalInterestCalculator.Compute(
            Principal, new DateOnly(2023, 1, 1), new DateOnly(2024, 1, 1), 8m, capitalize: true);

        capitalized.Interest.Amount.Should().Be(800m);
    }

    [Fact]
    public void Capitalized_OverTwoYears_Should_ExceedSimple_Anatocisme()
    {
        var from = new DateOnly(2023, 1, 1);
        var to = new DateOnly(2025, 1, 1);

        LegalInterestResult simple = BelgianLegalInterestCalculator.Compute(Principal, from, to, 8m);
        LegalInterestResult capitalized = BelgianLegalInterestCalculator.Compute(Principal, from, to, 8m, capitalize: true);

        capitalized.Interest.Amount.Should().BeGreaterThan(simple.Interest.Amount);
    }

    [Fact]
    public void NonPositivePeriod_Should_YieldZeroInterest()
    {
        LegalInterestResult result = BelgianLegalInterestCalculator.Compute(
            Principal, new DateOnly(2025, 1, 1), new DateOnly(2024, 1, 1), 8m);

        result.Interest.Amount.Should().Be(0m);
        result.Total.Amount.Should().Be(Principal.Amount);
    }
}
