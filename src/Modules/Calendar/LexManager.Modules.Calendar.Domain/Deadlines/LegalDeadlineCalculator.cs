using LexManager.Modules.Calendar.Domain.Common;

namespace LexManager.Modules.Calendar.Domain.Deadlines;

/// <summary>
/// Computes a procedural deadline from a base date (SRD Module 4: calcul des délais légaux).
/// Pure domain logic: applies the statutory delay for the deadline type and rolls the result to
/// the next business day when it lands on a weekend (a common procedural rule).
/// </summary>
public static class LegalDeadlineCalculator
{
    public static DateOnly Compute(DateOnly baseDate, LegalDeadlineType type)
    {
        DateOnly due = type switch
        {
            LegalDeadlineType.AppealAgainstJudgment => baseDate.AddMonths(1),
            LegalDeadlineType.OppositionDefault => baseDate.AddMonths(1),
            LegalDeadlineType.AppealAgainstOrder => baseDate.AddDays(15),
            LegalDeadlineType.Cassation => baseDate.AddMonths(2),
            _ => baseDate
        };

        return RollToBusinessDay(due);
    }

    private static DateOnly RollToBusinessDay(DateOnly date) => date.DayOfWeek switch
    {
        DayOfWeek.Saturday => date.AddDays(2),
        DayOfWeek.Sunday => date.AddDays(1),
        _ => date
    };
}
