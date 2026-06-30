namespace LexManager.Infrastructure.Retention;

/// <summary>
/// Computes legal retention expiry dates. Pure domain logic so the rules are unit-testable and
/// reused by both the background purge worker and any on-demand reporting.
/// </summary>
public static class RetentionPolicy
{
    public static int RetentionYears(RetentionCategory category) => category switch
    {
        RetentionCategory.StandardCase => 5,
        RetentionCategory.SpecificCase => 10,
        RetentionCategory.AccountingDocument => 10,
        RetentionCategory.ProspectData => 3,
        _ => 5
    };

    public static DateOnly ComputeExpiry(DateOnly referenceDate, RetentionCategory category) =>
        referenceDate.AddYears(RetentionYears(category));

    public static bool IsExpired(DateOnly referenceDate, RetentionCategory category, DateOnly asOf) =>
        asOf >= ComputeExpiry(referenceDate, category);
}
