using LexManager.Modules.CaseManagement.Domain.Cases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexManager.Modules.CaseManagement.Infrastructure.Persistence.Configurations;

internal sealed class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("cases");

        builder.HasKey(@case => @case.Id);

        builder.Property(@case => @case.Id)
            .HasConversion(id => id.Value, value => new CaseId(value))
            .ValueGeneratedNever();

        builder.Property(@case => @case.Title).HasMaxLength(256).IsRequired();
        builder.Property(@case => @case.ClientId).IsRequired();
        builder.Property(@case => @case.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(@case => @case.IsArchived).IsRequired();
        builder.Property(@case => @case.OpenedOnUtc).IsRequired();
        builder.Property(@case => @case.ClosedOnUtc);
        builder.Property(@case => @case.ArchivedOnUtc);

        builder.HasIndex(@case => @case.ClientId);

        builder.OwnsOne(@case => @case.Jurisdiction, jurisdiction =>
        {
            jurisdiction.Property(j => j.CourtName).HasColumnName("court_name").HasMaxLength(256);
            jurisdiction.Property(j => j.GeneralRegisterNumber).HasColumnName("rg_number").HasMaxLength(64);
            jurisdiction.Property(j => j.Judge).HasColumnName("judge").HasMaxLength(256);
        });
        builder.Navigation(@case => @case.Jurisdiction).IsRequired(false);

        builder.OwnsMany(@case => @case.AdverseParties, adverse =>
        {
            adverse.ToTable("case_adverse_parties");
            adverse.WithOwner().HasForeignKey("case_id");
            adverse.Property<int>("id");
            adverse.HasKey("id");
            adverse.Property(party => party.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            adverse.Property(party => party.Counsel).HasColumnName("counsel").HasMaxLength(256);
        });

        // Global query filter: archived cases are hidden from daily searches (SRD §5.3).
        builder.HasQueryFilter(@case => !@case.IsArchived);

        builder.Ignore(@case => @case.DomainEvents);
    }
}
