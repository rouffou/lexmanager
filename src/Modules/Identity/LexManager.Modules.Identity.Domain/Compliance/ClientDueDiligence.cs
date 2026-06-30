using LexManager.Modules.Identity.Domain.Compliance.Events;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Identity.Domain.Compliance;

/// <summary>
/// The anti-money-laundering due-diligence file for a client (devoir de vigilance LCB-FT,
/// SRD V11 §30). Aggregate root that collects the mandatory verification checks, computes a
/// regulatory compliance score, and gates acceptance of the mandate: a file can only be
/// approved once every required check for its risk profile is cleared (score = 100).
/// </summary>
public sealed class ClientDueDiligence : AggregateRoot<DueDiligenceId>
{
    private readonly List<VerificationCheck> _checks = [];

    private ClientDueDiligence() { }

    private ClientDueDiligence(DueDiligenceId id, Guid clientId, bool isLegalEntity, RiskLevel riskLevel) : base(id)
    {
        ClientId = clientId;
        IsLegalEntity = isLegalEntity;
        RiskLevel = riskLevel;
        Status = DueDiligenceStatus.InProgress;
        OpenedOnUtc = DateTime.UtcNow;
    }

    public Guid ClientId { get; private set; }
    public bool IsLegalEntity { get; private set; }
    public RiskLevel RiskLevel { get; private set; }
    public DueDiligenceStatus Status { get; private set; }
    public bool IsPoliticallyExposed { get; private set; }
    public DateTime OpenedOnUtc { get; private set; }
    public DateTime? DecidedOnUtc { get; private set; }
    public string? DecisionReason { get; private set; }

    public IReadOnlyList<VerificationCheck> Checks => _checks.AsReadOnly();

    /// <summary>
    /// The verification checks mandatory for this client's risk profile. Natural persons must show
    /// an identity document and proof of address; legal entities, a registry extract and their
    /// beneficial owners. Politically-exposed or high-risk clients additionally require enhanced
    /// vigilance (sanctions screening).
    /// </summary>
    public IReadOnlyList<VerificationKind> RequiredChecks
    {
        get
        {
            List<VerificationKind> required = IsLegalEntity
                ? [VerificationKind.CompanyRegistry, VerificationKind.BeneficialOwner]
                : [VerificationKind.IdentityDocument, VerificationKind.AddressProof];

            if (IsPoliticallyExposed || RiskLevel == RiskLevel.High)
            {
                required.Add(VerificationKind.SanctionsScreening);
            }

            return required;
        }
    }

    /// <summary>Regulatory compliance score (0–100): the share of required checks that are cleared.</summary>
    public int ComplianceScore
    {
        get
        {
            IReadOnlyList<VerificationKind> required = RequiredChecks;
            if (required.Count == 0)
            {
                return 100;
            }

            int cleared = required.Count(kind => _checks.Any(check => check.Kind == kind && check.Cleared));
            return (int)Math.Round(cleared * 100.0 / required.Count, MidpointRounding.AwayFromZero);
        }
    }

    public bool CanApprove => Status == DueDiligenceStatus.InProgress && ComplianceScore == 100;

    public static ClientDueDiligence Start(Guid clientId, bool isLegalEntity, RiskLevel riskLevel)
    {
        var file = new ClientDueDiligence(DueDiligenceId.New(), clientId, isLegalEntity, riskLevel);
        file.Raise(new DueDiligenceStartedDomainEvent(file.Id.Value, clientId, riskLevel));
        return file;
    }

    public void SetRiskLevel(RiskLevel riskLevel)
    {
        EnsureMutable();
        RiskLevel = riskLevel;
    }

    public void FlagPoliticallyExposed(bool isPoliticallyExposed)
    {
        EnsureMutable();
        IsPoliticallyExposed = isPoliticallyExposed;
    }

    public void RecordCheck(VerificationKind kind, string reference, bool cleared, string? notes = null)
    {
        EnsureMutable();

        if (string.IsNullOrWhiteSpace(reference))
        {
            throw new BusinessRuleValidationException(KycErrors.MissingReference);
        }

        VerificationCheck? existing = _checks.SingleOrDefault(check => check.Kind == kind);
        if (existing is null)
        {
            _checks.Add(new VerificationCheck(kind, reference.Trim(), cleared, notes));
        }
        else
        {
            existing.Update(reference.Trim(), cleared, notes);
        }
    }

    public void Approve()
    {
        EnsureMutable();

        if (!CanApprove)
        {
            throw new BusinessRuleValidationException(KycErrors.IncompleteDueDiligence);
        }

        Status = DueDiligenceStatus.Approved;
        DecidedOnUtc = DateTime.UtcNow;
        Raise(new DueDiligenceDecidedDomainEvent(Id.Value, ClientId, Status, ComplianceScore));
    }

    public void Reject(string reason)
    {
        EnsureMutable();

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleValidationException(KycErrors.MissingRejectionReason);
        }

        Status = DueDiligenceStatus.Rejected;
        DecisionReason = reason.Trim();
        DecidedOnUtc = DateTime.UtcNow;
        Raise(new DueDiligenceDecidedDomainEvent(Id.Value, ClientId, Status, ComplianceScore));
    }

    private void EnsureMutable()
    {
        if (Status != DueDiligenceStatus.InProgress)
        {
            throw new BusinessRuleValidationException(KycErrors.AlreadyDecided);
        }
    }
}
