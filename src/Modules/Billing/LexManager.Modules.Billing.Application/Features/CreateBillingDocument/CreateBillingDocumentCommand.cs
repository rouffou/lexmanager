using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.CreateBillingDocument;

public sealed record CreateBillingDocumentCommand(
    Guid CaseId,
    Guid ClientId,
    BillingDocumentKind Kind,
    BillingMode Mode,
    decimal TaxRatePercent = 21m, // Belgian VAT on lawyers' services (SRD §5)
    string Currency = Money.DefaultCurrency,
    VatRegime Regime = VatRegime.Standard) : ICommand<Result<Guid>>;
