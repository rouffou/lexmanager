namespace LexManager.Infrastructure.Retention;

/// <summary>Legal retention buckets and their statutory durations (SRD §5.3, RGPD).</summary>
public enum RetentionCategory
{
    /// <summary>Standard case: 5 years from closure (Art. 2224 Code civil).</summary>
    StandardCase = 0,

    /// <summary>Bodily injury / construction case: 10 years.</summary>
    SpecificCase = 1,

    /// <summary>Accounting pieces &amp; invoices: 10 years (fiscal/commercial obligation).</summary>
    AccountingDocument = 2,

    /// <summary>Prospect data without mandate: 3 years after last contact.</summary>
    ProspectData = 3
}

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
