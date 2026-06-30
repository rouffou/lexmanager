using LexManager.Modules.Identity.Domain.Compliance;
using LexManager.Modules.Identity.Domain.Compliance.Events;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Identity.UnitTests.Domain;

public class ClientDueDiligenceTests
{
    private static ClientDueDiligence StartPhysical(RiskLevel risk = RiskLevel.Standard) =>
        ClientDueDiligence.Start(Guid.NewGuid(), isLegalEntity: false, risk);

    private static ClientDueDiligence StartLegal(RiskLevel risk = RiskLevel.Standard) =>
        ClientDueDiligence.Start(Guid.NewGuid(), isLegalEntity: true, risk);

    [Fact]
    public void Start_Should_BeInProgress_WithZeroScore_AndRaiseEvent()
    {
        ClientDueDiligence file = StartPhysical();

        file.Status.Should().Be(DueDiligenceStatus.InProgress);
        file.ComplianceScore.Should().Be(0);
        file.CanApprove.Should().BeFalse();
        file.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<DueDiligenceStartedDomainEvent>();
    }

    [Fact]
    public void PhysicalPerson_Requires_IdentityDocument_And_AddressProof()
    {
        ClientDueDiligence file = StartPhysical();

        file.RequiredChecks.Should().BeEquivalentTo(
            [VerificationKind.IdentityDocument, VerificationKind.AddressProof]);
    }

    [Fact]
    public void LegalEntity_Requires_CompanyRegistry_And_BeneficialOwner()
    {
        ClientDueDiligence file = StartLegal();

        file.RequiredChecks.Should().BeEquivalentTo(
            [VerificationKind.CompanyRegistry, VerificationKind.BeneficialOwner]);
    }

    [Fact]
    public void HighRisk_Or_Pep_Adds_SanctionsScreening_AsEnhancedVigilance()
    {
        ClientDueDiligence highRisk = StartPhysical(RiskLevel.High);
        highRisk.RequiredChecks.Should().Contain(VerificationKind.SanctionsScreening);

        ClientDueDiligence pep = StartPhysical();
        pep.FlagPoliticallyExposed(true);
        pep.RequiredChecks.Should().Contain(VerificationKind.SanctionsScreening);
    }

    [Fact]
    public void ComplianceScore_Should_Reflect_ClearedRequiredChecks()
    {
        ClientDueDiligence file = StartPhysical();

        file.RecordCheck(VerificationKind.IdentityDocument, "PASS-123", cleared: true);
        file.ComplianceScore.Should().Be(50);

        file.RecordCheck(VerificationKind.AddressProof, "BILL-456", cleared: true);
        file.ComplianceScore.Should().Be(100);
        file.CanApprove.Should().BeTrue();
    }

    [Fact]
    public void UnclearedCheck_Should_NotCount_TowardsScore()
    {
        ClientDueDiligence file = StartPhysical();

        file.RecordCheck(VerificationKind.IdentityDocument, "PASS-123", cleared: true);
        file.RecordCheck(VerificationKind.AddressProof, "BILL-456", cleared: false);

        file.ComplianceScore.Should().Be(50);
        file.CanApprove.Should().BeFalse();
    }

    [Fact]
    public void RecordCheck_SameKindTwice_Should_Upsert_NotDuplicate()
    {
        ClientDueDiligence file = StartPhysical();

        file.RecordCheck(VerificationKind.IdentityDocument, "PASS-1", cleared: false);
        file.RecordCheck(VerificationKind.IdentityDocument, "PASS-1", cleared: true);

        file.Checks.Should().ContainSingle().Which.Cleared.Should().BeTrue();
    }

    [Fact]
    public void RecordCheck_EmptyReference_Should_Throw()
    {
        ClientDueDiligence file = StartPhysical();

        Action act = () => file.RecordCheck(VerificationKind.IdentityDocument, "  ", cleared: true);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(KycErrors.MissingReference);
    }

    [Fact]
    public void Approve_WithFullScore_Should_SetApproved_AndRaiseEvent()
    {
        ClientDueDiligence file = StartPhysical();
        file.RecordCheck(VerificationKind.IdentityDocument, "PASS-123", cleared: true);
        file.RecordCheck(VerificationKind.AddressProof, "BILL-456", cleared: true);
        file.ClearDomainEvents();

        file.Approve();

        file.Status.Should().Be(DueDiligenceStatus.Approved);
        file.DecidedOnUtc.Should().NotBeNull();
        file.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<DueDiligenceDecidedDomainEvent>()
            .Which.Status.Should().Be(DueDiligenceStatus.Approved);
    }

    [Fact]
    public void Approve_WithIncompleteChecks_Should_Throw()
    {
        ClientDueDiligence file = StartPhysical();
        file.RecordCheck(VerificationKind.IdentityDocument, "PASS-123", cleared: true);

        Action act = () => file.Approve();

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(KycErrors.IncompleteDueDiligence);
    }

    [Fact]
    public void Reject_Should_SetRejected_WithReason()
    {
        ClientDueDiligence file = StartPhysical();

        file.Reject("Documents falsifiés");

        file.Status.Should().Be(DueDiligenceStatus.Rejected);
        file.DecisionReason.Should().Be("Documents falsifiés");
    }

    [Fact]
    public void MutatingAfterDecision_Should_Throw_AlreadyDecided()
    {
        ClientDueDiligence file = StartPhysical();
        file.Reject("Refus");

        Action record = () => file.RecordCheck(VerificationKind.IdentityDocument, "X", cleared: true);
        Action approve = () => file.Approve();

        record.Should().Throw<BusinessRuleValidationException>().Which.Error.Should().Be(KycErrors.AlreadyDecided);
        approve.Should().Throw<BusinessRuleValidationException>().Which.Error.Should().Be(KycErrors.AlreadyDecided);
    }
}
