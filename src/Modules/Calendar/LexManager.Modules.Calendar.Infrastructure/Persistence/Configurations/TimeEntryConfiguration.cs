using LexManager.Modules.Calendar.Domain.TimeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexManager.Modules.Calendar.Infrastructure.Persistence.Configurations;

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
