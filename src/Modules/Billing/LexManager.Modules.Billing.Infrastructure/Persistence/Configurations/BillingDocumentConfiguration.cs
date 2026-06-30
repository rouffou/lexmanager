using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexManager.Modules.Billing.Infrastructure.Persistence.Configurations;

internal sealed class BillingDocumentConfiguration : IEntityTypeConfiguration<BillingDocument>
{
    public void Configure(EntityTypeBuilder<BillingDocument> builder)
    {
        builder.ToTable("billing_documents");

        builder.HasKey(document => document.Id);
        builder.Property(document => document.Id)
            .HasConversion(id => id.Value, value => new BillingDocumentId(value))
            .ValueGeneratedNever();

        builder.Property(document => document.CaseId).IsRequired();
        builder.Property(document => document.ClientId).IsRequired();
        builder.Property(document => document.Kind).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(document => document.Mode).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(document => document.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(document => document.TaxRatePercent).HasPrecision(5, 2).IsRequired();
        builder.Property(document => document.Currency).HasMaxLength(3).IsRequired();
        builder.Property(document => document.Number).HasMaxLength(32);
        builder.Property(document => document.CreatedOnUtc).IsRequired();
        builder.Property(document => document.IssuedOnUtc);
        builder.Property(document => document.DueDateUtc);
        builder.Property(document => document.PaidOnUtc);

        builder.HasIndex(document => document.CaseId);
        builder.HasIndex(document => document.Number);

        // Computed money values are derived from the lines, not stored.
        builder.Ignore(document => document.Subtotal);
        builder.Ignore(document => document.TaxAmount);
        builder.Ignore(document => document.Total);

        builder.OwnsMany(document => document.Lines, line =>
        {
            line.ToTable("billing_document_lines");
            line.WithOwner().HasForeignKey("billing_document_id");
            line.Property<int>("id");
            line.HasKey("id");
            line.Property(l => l.Description).HasColumnName("description").HasMaxLength(512).IsRequired();
            line.Property(l => l.Quantity).HasColumnName("quantity").HasPrecision(12, 2);
            line.Property(l => l.UnitPriceAmount).HasColumnName("unit_price").HasPrecision(14, 2);
            line.Property(l => l.Currency).HasColumnName("currency").HasMaxLength(3);
            line.Ignore(l => l.UnitPrice);
            line.Ignore(l => l.LineTotal);
        });

        builder.Navigation(document => document.Lines).AutoInclude();
        builder.Ignore(document => document.DomainEvents);
    }
}
