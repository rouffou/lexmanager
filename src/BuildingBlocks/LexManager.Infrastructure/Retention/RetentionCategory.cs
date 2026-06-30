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
