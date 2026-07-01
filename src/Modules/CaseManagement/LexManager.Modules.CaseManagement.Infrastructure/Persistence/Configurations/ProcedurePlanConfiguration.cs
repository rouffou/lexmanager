using LexManager.Modules.CaseManagement.Domain.Procedures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexManager.Modules.CaseManagement.Infrastructure.Persistence.Configurations;

internal sealed class ProcedurePlanConfiguration : IEntityTypeConfiguration<ProcedurePlan>
{
    public void Configure(EntityTypeBuilder<ProcedurePlan> builder)
    {
        builder.ToTable("procedure_plans");

        builder.HasKey(plan => plan.Id);

        builder.Property(plan => plan.Id)
            .HasConversion(id => id.Value, value => new ProcedurePlanId(value))
            .ValueGeneratedNever();

        builder.Property(plan => plan.CaseId).IsRequired();
        builder.Property(plan => plan.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(plan => plan.ReferenceOnUtc).IsRequired();
        builder.Property(plan => plan.CreatedOnUtc).IsRequired();

        // One procedure tree per case.
        builder.HasIndex(plan => plan.CaseId).IsUnique();

        builder.Ignore(plan => plan.CurrentStage);
        builder.Ignore(plan => plan.TotalStages);
        builder.Ignore(plan => plan.ResolvedStages);
        builder.Ignore(plan => plan.ProgressPercent);
        builder.Ignore(plan => plan.IsComplete);

        builder.OwnsMany(plan => plan.Stages, stage =>
        {
            stage.ToTable("procedure_stages");
            stage.WithOwner().HasForeignKey("procedure_plan_id");
            stage.HasKey("procedure_plan_id", nameof(ProcedureStage.Order));
            stage.Property(s => s.Order).HasColumnName("stage_order");
            stage.Property(s => s.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            stage.Property(s => s.Phase).HasColumnName("phase").HasMaxLength(64).IsRequired();
            stage.Property(s => s.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(16).IsRequired();
            stage.Property(s => s.PlannedOnUtc).HasColumnName("planned_on_utc");
            stage.Property(s => s.CompletedOnUtc).HasColumnName("completed_on_utc");
        });

        builder.Navigation(plan => plan.Stages).AutoInclude();

        builder.Ignore(plan => plan.DomainEvents);
    }
}
