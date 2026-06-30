using LexManager.Modules.Documents.Domain.Documents;
using LexManager.Modules.Documents.Domain.Documents.Events;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Documents.UnitTests.Domain;

public class DocumentTests
{
    private static Document NewDocument() => Document.Create(
        Guid.NewGuid(), "conclusions.pdf", "application/pdf", DocumentCategory.Conclusions,
        storageKey: "2026/06/abc", sizeBytes: 1024, checksum: "deadbeef");

    [Fact]
    public void Create_Should_StartAtVersionOne_AndRaiseEvent()
    {
        Document document = NewDocument();

        document.CurrentVersionNumber.Should().Be(1);
        document.Versions.Should().ContainSingle();
        document.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<DocumentUploadedDomainEvent>();
    }

    [Fact]
    public void Create_Should_Throw_WhenFileNameMissing()
    {
        Action act = () => Document.Create(Guid.NewGuid(), "  ", "application/pdf",
            DocumentCategory.Other, "k", 1, "c");

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(DocumentErrors.MissingFileName);
    }

    [Fact]
    public void AddVersion_Should_IncrementVersionNumber_AndRaiseEvent()
    {
        Document document = NewDocument();
        document.ClearDomainEvents();

        DocumentVersion version = document.AddVersion("2026/06/def", 2048, "cafef00d");

        version.VersionNumber.Should().Be(2);
        document.CurrentVersionNumber.Should().Be(2);
        document.Versions.Should().HaveCount(2);
        document.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<DocumentVersionAddedDomainEvent>();
    }

    [Fact]
    public void CurrentVersion_Should_ReturnLatest()
    {
        Document document = NewDocument();
        document.AddVersion("2026/06/def", 2048, "cafef00d");

        document.CurrentVersion.VersionNumber.Should().Be(2);
        document.CurrentVersion.StorageKey.Should().Be("2026/06/def");
    }
}
