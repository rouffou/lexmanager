namespace LexManager.Modules.Calendar.Domain.Common;

public readonly record struct TimeEntryId(Guid Value)
{
    public static TimeEntryId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
