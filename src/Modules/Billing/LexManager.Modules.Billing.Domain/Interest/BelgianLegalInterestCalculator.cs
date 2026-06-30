using LexManager.Modules.Billing.Domain.Common;

namespace LexManager.Modules.Billing.Domain.Interest;

/// <summary>Outcome of a Belgian legal-interest simulation.</summary>
public sealed record LegalInterestResult(
    Money Principal,
    DateOnly FromDate,
    DateOnly ToDate,
    int Days,
    decimal AnnualRatePercent,
    bool Capitalized,
    Money Interest,
    Money Total);

/// <summary>
/// Computes default interest on a monetary condemnation at the Belgian statutory rate
/// (SRD V11 §5: simulateur d'intérêts légaux). Supports annual capitalization — anatocisme,
/// art. 1154 du Code civil — where accrued interest is added to the principal each full year.
/// Pure domain logic, day-count Actual/365.
/// </summary>
public static class BelgianLegalInterestCalculator
{
    private const int DaysPerYear = 365;

    public static LegalInterestResult Compute(
        Money principal,
        DateOnly fromDate,
        DateOnly toDate,
        decimal annualRatePercent,
        bool capitalize = false)
    {
        int totalDays = toDate.DayNumber - fromDate.DayNumber;
        if (totalDays <= 0 || annualRatePercent <= 0)
        {
            return new LegalInterestResult(principal, fromDate, toDate, Math.Max(totalDays, 0),
                annualRatePercent, capitalize, Money.Zero(principal.Currency), principal);
        }

        Money interest = capitalize
            ? Capitalized(principal, fromDate, toDate, annualRatePercent)
            : principal.Multiply(annualRatePercent / 100m).Multiply(totalDays / (decimal)DaysPerYear);

        return new LegalInterestResult(
            principal, fromDate, toDate, totalDays, annualRatePercent, capitalize, interest, principal.Add(interest));
    }

    private static Money Capitalized(Money principal, DateOnly fromDate, DateOnly toDate, decimal annualRatePercent)
    {
        decimal rate = annualRatePercent / 100m;
        Money balance = principal;
        DateOnly cursor = fromDate;

        // Compound at each anniversary; apply the partial final period without compounding.
        while (cursor.AddYears(1) <= toDate)
        {
            DateOnly next = cursor.AddYears(1);
            int yearDays = next.DayNumber - cursor.DayNumber;
            balance = balance.Add(balance.Multiply(rate).Multiply(yearDays / (decimal)DaysPerYear));
            cursor = next;
        }

        int remainingDays = toDate.DayNumber - cursor.DayNumber;
        if (remainingDays > 0)
        {
            balance = balance.Add(balance.Multiply(rate).Multiply(remainingDays / (decimal)DaysPerYear));
        }

        return balance.Add(principal.Multiply(-1m)); // interest = compounded balance − principal
    }
}
