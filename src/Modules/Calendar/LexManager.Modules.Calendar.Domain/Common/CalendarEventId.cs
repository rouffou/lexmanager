namespace LexManager.Modules.Calendar.Domain.Common;

public readonly record struct CalendarEventId(Guid Value)
{
    public static CalendarEventId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
