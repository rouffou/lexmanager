using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.Calendar.Domain.TimeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexManager.Modules.Calendar.Infrastructure.Persistence.Configurations;

internal sealed class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("calendar_events");

        builder.HasKey(calendarEvent => calendarEvent.Id);
        builder.Property(calendarEvent => calendarEvent.Id)
            .HasConversion(id => id.Value, value => new Domain.Common.CalendarEventId(value))
            .ValueGeneratedNever();

        builder.Property(calendarEvent => calendarEvent.OwnerUserId).IsRequired();
        builder.Property(calendarEvent => calendarEvent.Title).HasMaxLength(256).IsRequired();
        builder.Property(calendarEvent => calendarEvent.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(calendarEvent => calendarEvent.CaseId);
        builder.Property(calendarEvent => calendarEvent.Location).HasMaxLength(512);
        builder.Property(calendarEvent => calendarEvent.IsPrivate).IsRequired();
        builder.Property(calendarEvent => calendarEvent.CreatedOnUtc).IsRequired();
        builder.Property(calendarEvent => calendarEvent.Provider).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(calendarEvent => calendarEvent.ExternalId).HasMaxLength(256);
        builder.Property(calendarEvent => calendarEvent.ExternalETag).HasMaxLength(256);

        builder.OwnsOne(calendarEvent => calendarEvent.Period, period =>
        {
            period.Property(p => p.StartUtc).HasColumnName("start_utc").IsRequired();
            period.Property(p => p.EndUtc).HasColumnName("end_utc").IsRequired();
        });
        builder.Navigation(calendarEvent => calendarEvent.Period).IsRequired();

        builder.HasIndex(calendarEvent => calendarEvent.OwnerUserId);
        builder.HasIndex(calendarEvent => calendarEvent.CaseId);

        builder.Ignore(calendarEvent => calendarEvent.DomainEvents);
    }
}

internal sealed class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("time_entries");

        builder.HasKey(timeEntry => timeEntry.Id);
        builder.Property(timeEntry => timeEntry.Id)
            .HasConversion(id => id.Value, value => new Domain.Common.TimeEntryId(value))
            .ValueGeneratedNever();

        builder.Property(timeEntry => timeEntry.CaseId).IsRequired();
        builder.Property(timeEntry => timeEntry.UserId).IsRequired();
        builder.Property(timeEntry => timeEntry.Description).HasMaxLength(1024);
        builder.Property(timeEntry => timeEntry.WorkedOnUtc).IsRequired();
        builder.Property(timeEntry => timeEntry.DurationMinutes).IsRequired();
        builder.Property(timeEntry => timeEntry.IsBillable).IsRequired();
        builder.Property(timeEntry => timeEntry.CreatedOnUtc).IsRequired();

        builder.HasIndex(timeEntry => timeEntry.CaseId);

        builder.Ignore(timeEntry => timeEntry.DomainEvents);
    }
}
