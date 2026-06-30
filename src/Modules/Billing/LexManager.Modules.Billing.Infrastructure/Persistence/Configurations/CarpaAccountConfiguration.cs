using LexManager.Modules.Billing.Domain.Carpa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexManager.Modules.Billing.Infrastructure.Persistence.Configurations;

internal sealed class CarpaAccountConfiguration : IEntityTypeConfiguration<CarpaAccount>
{
    public void Configure(EntityTypeBuilder<CarpaAccount> builder)
    {
        builder.ToTable("carpa_accounts");

        builder.HasKey(account => account.Id);
        builder.Property(account => account.Id)
            .HasConversion(id => id.Value, value => new CarpaAccountId(value))
            .ValueGeneratedNever();

        builder.Property(account => account.CaseId).IsRequired();
        builder.Property(account => account.ClientId).IsRequired();
        builder.Property(account => account.Currency).HasMaxLength(3).IsRequired();
        builder.Property(account => account.OpenedOnUtc).IsRequired();

        // One third-party account per case (AVOCATS.BE / OVB traceability).
        builder.HasIndex(account => account.CaseId).IsUnique();
        builder.HasIndex(account => account.ClientId);

        // Balance is derived from the movements, never stored.
        builder.Ignore(account => account.Balance);

        builder.OwnsMany(account => account.Transactions, transaction =>
        {
            transaction.ToTable("carpa_transactions");
            transaction.WithOwner().HasForeignKey("carpa_account_id");
            transaction.Property<int>("id");
            transaction.HasKey("id");
            transaction.Property(movement => movement.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(16).IsRequired();
            transaction.Property(movement => movement.AmountValue).HasColumnName("amount").HasPrecision(14, 2).IsRequired();
            transaction.Property(movement => movement.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
            transaction.Property(movement => movement.Description).HasColumnName("description").HasMaxLength(512).IsRequired();
            transaction.Property(movement => movement.Counterparty).HasColumnName("counterparty").HasMaxLength(256);
            transaction.Property(movement => movement.OccurredOnUtc).HasColumnName("occurred_on_utc").IsRequired();
            transaction.Ignore(movement => movement.Amount);
            transaction.Ignore(movement => movement.SignedAmount);
        });

        builder.Navigation(account => account.Transactions).AutoInclude();
        builder.Ignore(account => account.DomainEvents);
    }
}
