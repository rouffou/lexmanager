using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.Domain.Common;
using Mediarq.UnitOfWork;

namespace LexManager.Modules.Billing.Application.Abstractions;

/// <summary>Module-scoped unit of work (Mediarq's <see cref="IUnitOfWork"/>) for Billing.</summary>
public interface IBillingUnitOfWork : IUnitOfWork;

/// <summary>Generates the next legal document number for a given kind (e.g. FAC-2026-000123).</summary>
public interface IInvoiceNumberGenerator
{
    Task<string> NextAsync(BillingDocumentKind kind, CancellationToken cancellationToken = default);
}

/// <summary>Sends a payment reminder for an overdue document (SRD Module 5: relances automatiques).</summary>
public interface IPaymentReminderSender
{
    Task SendAsync(Guid documentId, Guid clientId, decimal amount, CancellationToken cancellationToken = default);
}

/// <summary>Read-side port (CQRS) returning flat DTOs.</summary>
public interface IBillingReadRepository
{
    Task<BillingDocumentResponse?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<PagedList<BillingDocumentSummaryResponse>> GetByCaseAsync(
        Guid caseId,
        PaginationParameters parameters,
        CancellationToken cancellationToken = default);

    Task<CaseBillingSummaryResponse> GetCaseBillingSummaryAsync(Guid caseId, CancellationToken cancellationToken = default);
}
