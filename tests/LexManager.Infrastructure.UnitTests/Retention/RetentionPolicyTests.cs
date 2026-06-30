using LexManager.Infrastructure.Retention;

namespace LexManager.Infrastructure.UnitTests.Retention;

public class RetentionPolicyTests
{
    [Theory]
    [InlineData(RetentionCategory.StandardCase, 5)]
    [InlineData(RetentionCategory.SpecificCase, 10)]
    [InlineData(RetentionCategory.AccountingDocument, 10)]
    [InlineData(RetentionCategory.ProspectData, 3)]
    public void ComputeExpiry_Should_AddStatutoryYears(RetentionCategory category, int years)
    {
        var reference = new DateOnly(2026, 6, 30);

        DateOnly expiry = RetentionPolicy.ComputeExpiry(reference, category);

        expiry.Should().Be(reference.AddYears(years));
    }

    [Fact]
    public void IsExpired_Should_BeFalse_BeforeExpiry_AndTrue_After()
    {
        var closure = new DateOnly(2020, 1, 1);

        RetentionPolicy.IsExpired(closure, RetentionCategory.StandardCase, new DateOnly(2024, 12, 31)).Should().BeFalse();
        RetentionPolicy.IsExpired(closure, RetentionCategory.StandardCase, new DateOnly(2025, 1, 1)).Should().BeTrue();
    }
}
