using LexManager.Modules.Billing.Domain.Common;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Billing.Domain.Billing;

/// <summary>
/// A financial document for a case (invoice, provision call or fee note). Aggregate root governing
/// the billing lifecycle draft → issued → paid/overdue and the computation of legal totals
/// (subtotal, VAT, total) — SRD Module 5.
/// </summary>
public sealed class BillingDocument : AggregateRoot<BillingDocumentId>
{
    private readonly List<InvoiceLine> _lines = [];

    private BillingDocument() { }

    private BillingDocument(
        BillingDocumentId id,
        Guid caseId,
        Guid clientId,
        BillingDocumentKind kind,
        BillingMode mode,
        decimal taxRatePercent,
        string currency) : base(id)
    {
        CaseId = caseId;
        ClientId = clientId;
        Kind = kind;
        Mode = mode;
        TaxRatePercent = taxRatePercent;
        Currency = currency;
        Status = BillingStatus.Draft;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid CaseId { get; private set; }
    public Guid ClientId { get; private set; }
    public BillingDocumentKind Kind { get; private set; }
    public BillingMode Mode { get; private set; }
    public BillingStatus Status { get; private set; }
    public decimal TaxRatePercent { get; private set; }
    public string Currency { get; private set; } = Money.DefaultCurrency;
    public string? Number { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? IssuedOnUtc { get; private set; }
    public DateTime? DueDateUtc { get; private set; }
    public DateTime? PaidOnUtc { get; private set; }

    public IReadOnlyList<InvoiceLine> Lines => _lines.AsReadOnly();

    public Money Subtotal => _lines.Aggregate(Money.Zero(Currency), (sum, line) => sum.Add(line.LineTotal));
    public Money TaxAmount => Subtotal.Percentage(TaxRatePercent);
    public Money Total => Subtotal.Add(TaxAmount);

    public static BillingDocument CreateDraft(
        Guid caseId,
        Guid clientId,
        BillingDocumentKind kind,
        BillingMode mode,
        decimal taxRatePercent,
        string currency = Money.DefaultCurrency)
    {
        return new BillingDocument(
            BillingDocumentId.New(), caseId, clientId, kind, mode, taxRatePercent,
            string.IsNullOrWhiteSpace(currency) ? Money.DefaultCurrency : currency.ToUpperInvariant());
    }

    public void AddLine(string description, decimal quantity, Money unitPrice)
    {
        EnsureDraft();
        if (unitPrice.Currency != Currency)
        {
            throw new BusinessRuleValidationException(BillingErrors.CurrencyMismatch);
        }

        _lines.Add(new InvoiceLine(string.IsNullOrWhiteSpace(description) ? "Prestation" : description.Trim(), quantity, unitPrice));
    }

    public void Issue(string number, DateTime dueDateUtc)
    {
        EnsureDraft();
        if (_lines.Count == 0)
        {
            throw new BusinessRuleValidationException(BillingErrors.CannotIssueEmpty);
        }

        Number = number;
        Status = BillingStatus.Issued;
        IssuedOnUtc = DateTime.UtcNow;
        DueDateUtc = dueDateUtc;

        Raise(new BillingDocumentIssuedDomainEvent(Id.Value, CaseId, number, Total.Amount));
    }

    public void RegisterPayment()
    {
        if (Status is not (BillingStatus.Issued or BillingStatus.Overdue))
        {
            throw new BusinessRuleValidationException(BillingErrors.NotIssued);
        }

        Status = BillingStatus.Paid;
        PaidOnUtc = DateTime.UtcNow;
        Raise(new PaymentRegisteredDomainEvent(Id.Value, Total.Amount, PaidOnUtc.Value));
    }

    public bool MarkOverdueIfDue(DateTime asOfUtc)
    {
        if (Status == BillingStatus.Issued && DueDateUtc is { } due && due < asOfUtc)
        {
            Status = BillingStatus.Overdue;
            return true;
        }

        return false;
    }

    public void Cancel()
    {
        if (Status is BillingStatus.Paid)
        {
            throw new BusinessRuleValidationException(BillingErrors.NotEditable);
        }

        Status = BillingStatus.Cancelled;
    }

    private void EnsureDraft()
    {
        if (Status != BillingStatus.Draft)
        {
            throw new BusinessRuleValidationException(BillingErrors.NotEditable);
        }
    }
}
