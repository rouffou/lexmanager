using LexManager.Modules.Documents.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LexManager.Modules.Documents.Infrastructure.Persistence.Configurations;

internal sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");

        builder.HasKey(document => document.Id);

        builder.Property(document => document.Id)
            .HasConversion(id => id.Value, value => new DocumentId(value))
            .ValueGeneratedNever();

        builder.Property(document => document.CaseId).IsRequired();
        builder.Property(document => document.FileName).HasMaxLength(512).IsRequired();
        builder.Property(document => document.ContentType).HasMaxLength(256).IsRequired();
        builder.Property(document => document.Category).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(document => document.IsConfidential).IsRequired();
        builder.Property(document => document.CreatedOnUtc).IsRequired();

        // OCR-extracted body feeding full-text search (SRD §7.2). Unbounded text column.
        builder.Property(document => document.ExtractedText).HasColumnType("text");

        builder.HasIndex(document => document.CaseId);
        builder.Ignore(document => document.CurrentVersionNumber);
        builder.Ignore(document => document.CurrentVersion);
        builder.Ignore(document => document.IsIndexed);

        builder.OwnsMany(document => document.Versions, version =>
        {
            version.ToTable("document_versions");
            version.WithOwner().HasForeignKey("document_id");
            version.HasKey("document_id", nameof(DocumentVersion.VersionNumber));
            version.Property(v => v.VersionNumber).HasColumnName("version_number");
            version.Property(v => v.StorageKey).HasColumnName("storage_key").HasMaxLength(512).IsRequired();
            version.Property(v => v.SizeBytes).HasColumnName("size_bytes");
            version.Property(v => v.Checksum).HasColumnName("checksum").HasMaxLength(128).IsRequired();
            version.Property(v => v.UploadedOnUtc).HasColumnName("uploaded_on_utc");
        });

        builder.Navigation(document => document.Versions).AutoInclude();

        builder.Ignore(document => document.DomainEvents);
    }
}
