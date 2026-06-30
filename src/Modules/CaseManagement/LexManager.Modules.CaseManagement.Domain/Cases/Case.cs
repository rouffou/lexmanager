using LexManager.Modules.CaseManagement.Domain.Cases.Events;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.CaseManagement.Domain.Cases;

/// <summary>
/// A case file (dossier). Aggregate root governing the lifecycle ouverture → instruction → clôture,
/// and the intermediate archive used for legal retention (SRD §5.3). Linked to a client by id only —
/// the client itself lives in the Identity module (SRD §3.2).
/// </summary>
public sealed class Case : AggregateRoot<CaseId>
{
    private readonly List<AdverseParty> _adverseParties = [];

    private Case() { }

    private Case(CaseId id, string title, Guid clientId, Jurisdiction? jurisdiction) : base(id)
    {
        Title = title;
        ClientId = clientId;
        Jurisdiction = jurisdiction;
        Status = CaseStatus.Opened;
        IsArchived = false;
        OpenedOnUtc = DateTime.UtcNow;
    }

    public string Title { get; private set; } = null!;
    public Guid ClientId { get; private set; }
    public CaseStatus Status { get; private set; }
    public Jurisdiction? Jurisdiction { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTime OpenedOnUtc { get; private set; }
    public DateTime? ClosedOnUtc { get; private set; }
    public DateTime? ArchivedOnUtc { get; private set; }

    public IReadOnlyCollection<AdverseParty> AdverseParties => _adverseParties.AsReadOnly();

    public static Case Open(string title, Guid clientId, Jurisdiction? jurisdiction = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new BusinessRuleValidationException(CaseErrors.EmptyTitle);
        }

        var @case = new Case(CaseId.New(), title.Trim(), clientId, jurisdiction);
        @case.Raise(new CaseOpenedDomainEvent(@case.Id.Value, clientId, @case.Title));
        return @case;
    }

    public void MoveToInvestigation()
    {
        EnsureNotClosed();
        Status = CaseStatus.UnderInvestigation;
    }

    public void AddAdverseParty(AdverseParty adverseParty)
    {
        EnsureNotClosed();
        if (!_adverseParties.Contains(adverseParty))
        {
            _adverseParties.Add(adverseParty);
        }
    }

    public void AssignJurisdiction(Jurisdiction jurisdiction)
    {
        EnsureNotClosed();
        Jurisdiction = jurisdiction;
    }

    public void Close()
    {
        EnsureNotClosed();
        Status = CaseStatus.Closed;
        ClosedOnUtc = DateTime.UtcNow;
        Raise(new CaseClosedDomainEvent(Id.Value, ClosedOnUtc.Value));
    }

    /// <summary>Moves a closed case into the intermediate archive (excluded from daily searches, SRD §5.3).</summary>
    public void Archive()
    {
        if (Status != CaseStatus.Closed)
        {
            throw new BusinessRuleValidationException(CaseErrors.CannotArchiveBeforeClosing);
        }

        IsArchived = true;
        ArchivedOnUtc = DateTime.UtcNow;
    }

    private void EnsureNotClosed()
    {
        if (Status == CaseStatus.Closed)
        {
            throw new BusinessRuleValidationException(CaseErrors.AlreadyClosed);
        }
    }
}
