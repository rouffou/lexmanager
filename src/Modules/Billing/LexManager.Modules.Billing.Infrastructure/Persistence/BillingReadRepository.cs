using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Billing.Infrastructure.Persistence;

internal sealed class BillingReadRepository(BillingDbContext context) : IBillingReadRepository
{
    public async Task<BillingDocumentResponse?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var id = new BillingDocumentId(documentId);

        BillingDocument? document = await context.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        return document is null ? null : Map(document);
    }

    public async Task<PagedList<BillingDocumentSummaryResponse>> GetByCaseAsync(
        Guid caseId,
        PaginationParameters parameters,
        CancellationToken cancellationToken = default)
    {
        IQueryable<BillingDocument> query = context.Documents.AsNoTracking().Where(d => d.CaseId == caseId);

        int totalCount = await query.CountAsync(cancellationToken);

        List<BillingDocument> documents = await query
            .OrderByDescending(d => d.CreatedOnUtc)
            .Skip(parameters.Skip)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        IReadOnlyList<BillingDocumentSummaryResponse> items = documents
            .Select(d => new BillingDocumentSummaryResponse(
                d.Id.Value, d.Kind.ToString(), d.Status.ToString(), d.Number, d.Total.Amount, d.Currency, d.DueDateUtc, d.CreatedOnUtc))
            .ToList();

        return new PagedList<BillingDocumentSummaryResponse>(items, parameters.Page, parameters.PageSize, totalCount);
    }

    public async Task<CaseBillingSummaryResponse> GetCaseBillingSummaryAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        List<BillingDocument> documents = await context.Documents
            .AsNoTracking()
            .Where(d => d.CaseId == caseId && d.Status != BillingStatus.Draft && d.Status != BillingStatus.Cancelled)
            .ToListAsync(cancellationToken);

        decimal invoiced = documents.Sum(d => d.Total.Amount);
        decimal paid = documents.Where(d => d.Status == BillingStatus.Paid).Sum(d => d.Total.Amount);
        string currency = documents.FirstOrDefault()?.Currency ?? Money.DefaultCurrency;

        return new CaseBillingSummaryResponse(caseId, invoiced, paid, invoiced - paid, currency, documents.Count);
    }

    private static BillingDocumentResponse Map(BillingDocument document) => new(
        document.Id.Value,
        document.CaseId,
        document.ClientId,
        document.Kind.ToString(),
        document.Mode.ToString(),
        document.Status.ToString(),
        document.VatRegime.ToString(),
        document.LegalMention,
        document.Number,
        document.Currency,
        document.Subtotal.Amount,
        document.TaxRatePercent,
        document.TaxAmount.Amount,
        document.Total.Amount,
        document.Lines.Select(line => new InvoiceLineResponse(line.Description, line.Quantity, line.UnitPriceAmount, line.LineTotal.Amount)).ToList(),
        document.IssuedOnUtc,
        document.DueDateUtc,
        document.PaidOnUtc,
        document.CreatedOnUtc);
}
