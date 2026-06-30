using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Calendar.Domain.Common;

/// <summary>A half-open time interval [Start, End). Used for agenda slots and overlap detection.</summary>
public sealed class DateRange : ValueObject
{
    private DateRange(DateTime startUtc, DateTime endUtc)
    {
        StartUtc = startUtc;
        EndUtc = endUtc;
    }

    public DateTime StartUtc { get; }
    public DateTime EndUtc { get; }
    public TimeSpan Duration => EndUtc - StartUtc;

    public static DateRange Create(DateTime startUtc, DateTime endUtc)
    {
        if (endUtc <= startUtc)
        {
            throw new BusinessRuleValidationException(CalendarErrors.InvalidTimeRange);
        }

        return new DateRange(DateTime.SpecifyKind(startUtc, DateTimeKind.Utc), DateTime.SpecifyKind(endUtc, DateTimeKind.Utc));
    }

    /// <summary>True if the two ranges share any instant (touching boundaries do not overlap).</summary>
    public bool Overlaps(DateRange other) => StartUtc < other.EndUtc && other.StartUtc < EndUtc;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartUtc;
        yield return EndUtc;
    }
}
