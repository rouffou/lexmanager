using LexManager.Modules.Identity.Domain.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexManager.Modules.Identity.Infrastructure.Persistence.Configurations;

internal sealed class ClientDueDiligenceConfiguration : IEntityTypeConfiguration<ClientDueDiligence>
{
    public void Configure(EntityTypeBuilder<ClientDueDiligence> builder)
    {
        builder.ToTable("client_due_diligence");

        builder.HasKey(file => file.Id);
        builder.Property(file => file.Id)
            .HasConversion(id => id.Value, value => new DueDiligenceId(value))
            .ValueGeneratedNever();

        builder.Property(file => file.ClientId).IsRequired();
        builder.Property(file => file.IsLegalEntity).IsRequired();
        builder.Property(file => file.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(file => file.RiskLevel).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(file => file.IsPoliticallyExposed).IsRequired();
        builder.Property(file => file.OpenedOnUtc).IsRequired();
        builder.Property(file => file.DecidedOnUtc);
        builder.Property(file => file.DecisionReason).HasMaxLength(1024);

        // One due-diligence file per client.
        builder.HasIndex(file => file.ClientId).IsUnique();

        // Computed compliance values are derived from the checks, not stored.
        builder.Ignore(file => file.RequiredChecks);
        builder.Ignore(file => file.ComplianceScore);
        builder.Ignore(file => file.CanApprove);

        builder.OwnsMany(file => file.Checks, check =>
        {
            check.ToTable("client_verification_checks");
            check.WithOwner().HasForeignKey("due_diligence_id");
            check.Property<int>("id");
            check.HasKey("id");
            check.Property(c => c.Kind).HasColumnName("kind").HasConversion<string>().HasMaxLength(32).IsRequired();
            check.Property(c => c.Reference).HasColumnName("reference").HasMaxLength(256).IsRequired();
            check.Property(c => c.Cleared).HasColumnName("cleared").IsRequired();
            check.Property(c => c.Notes).HasColumnName("notes").HasMaxLength(1024);
            check.Property(c => c.RecordedOnUtc).HasColumnName("recorded_on_utc").IsRequired();
        });

        builder.Navigation(file => file.Checks).AutoInclude();
        builder.Ignore(file => file.DomainEvents);
    }
}
