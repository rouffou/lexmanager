using LexManager.Modules.Identity.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexManager.Modules.Identity.Infrastructure.Persistence.Configurations;

internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");

        builder.HasKey(client => client.Id);

        builder.Property(client => client.Id)
            .HasConversion(id => id.Value, value => new ClientId(value))
            .ValueGeneratedNever();

        builder.Property(client => client.Type)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(client => client.Email)
            .HasConversion(email => email.Value, value => Email.Create(value))
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(client => client.Email).IsUnique();

        builder.Property(client => client.Phone).HasMaxLength(32);
        builder.Property(client => client.CreatedOnUtc).IsRequired();
        builder.Property(client => client.NationalIdentityNumber).HasMaxLength(64);

        // Natural-person name (optional owned value object).
        builder.OwnsOne(client => client.Name, name =>
        {
            name.Property(personName => personName.FirstName).HasColumnName("first_name").HasMaxLength(128);
            name.Property(personName => personName.LastName).HasColumnName("last_name").HasMaxLength(128);
        });

        // Legal-entity details (optional owned value object).
        builder.OwnsOne(client => client.Organization, organization =>
        {
            organization.Property(org => org.CompanyName).HasColumnName("company_name").HasMaxLength(256);
            organization.Property(org => org.RegistrationNumber).HasColumnName("registration_number").HasMaxLength(14);
            organization.Property(org => org.LegalRepresentative).HasColumnName("legal_representative").HasMaxLength(256);
        });

        builder.Navigation(client => client.Name).IsRequired(false);
        builder.Navigation(client => client.Organization).IsRequired(false);

        builder.Ignore(client => client.DomainEvents);
    }
}
