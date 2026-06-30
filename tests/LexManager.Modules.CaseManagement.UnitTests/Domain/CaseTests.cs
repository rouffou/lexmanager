using LexManager.Modules.CaseManagement.Domain.Cases;
using LexManager.Modules.CaseManagement.Domain.Cases.Events;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.CaseManagement.UnitTests.Domain;

public class CaseTests
{
    [Fact]
    public void Open_Should_StartOpened_AndRaiseEvent()
    {
        var clientId = Guid.NewGuid();

        Case @case = Case.Open("Litige Commercial SNE", clientId);

        @case.Status.Should().Be(CaseStatus.Opened);
        @case.ClientId.Should().Be(clientId);
        @case.IsArchived.Should().BeFalse();
        @case.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<CaseOpenedDomainEvent>();
    }

    [Fact]
    public void Open_Should_Throw_WhenTitleEmpty()
    {
        Action act = () => Case.Open("   ", Guid.NewGuid());

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(CaseErrors.EmptyTitle);
    }

    [Fact]
    public void Close_Should_SetClosed_AndRaiseEvent()
    {
        Case @case = Case.Open("Dossier", Guid.NewGuid());
        @case.ClearDomainEvents();

        @case.Close();

        @case.Status.Should().Be(CaseStatus.Closed);
        @case.ClosedOnUtc.Should().NotBeNull();
        @case.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<CaseClosedDomainEvent>();
    }

    [Fact]
    public void Close_Twice_Should_Throw()
    {
        Case @case = Case.Open("Dossier", Guid.NewGuid());
        @case.Close();

        Action act = () => @case.Close();

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(CaseErrors.AlreadyClosed);
    }

    [Fact]
    public void Archive_Should_Throw_WhenCaseNotClosed()
    {
        Case @case = Case.Open("Dossier", Guid.NewGuid());

        Action act = () => @case.Archive();

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(CaseErrors.CannotArchiveBeforeClosing);
    }

    [Fact]
    public void Archive_Should_Succeed_AfterClosing()
    {
        Case @case = Case.Open("Dossier", Guid.NewGuid());
        @case.Close();

        @case.Archive();

        @case.IsArchived.Should().BeTrue();
        @case.ArchivedOnUtc.Should().NotBeNull();
    }

    [Fact]
    public void AddAdverseParty_Should_BeIdempotent()
    {
        Case @case = Case.Open("Dossier", Guid.NewGuid());
        var party = AdverseParty.Create("Société X", "Maître Y");

        @case.AddAdverseParty(party);
        @case.AddAdverseParty(AdverseParty.Create("Société X", "Maître Y"));

        @case.AdverseParties.Should().ContainSingle();
    }

    [Fact]
    public void Jurisdiction_Create_Should_Throw_WhenRgNumberMissing()
    {
        Action act = () => Jurisdiction.Create("Tribunal judiciaire", "  ");

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(CaseErrors.InvalidJurisdiction);
    }
}
