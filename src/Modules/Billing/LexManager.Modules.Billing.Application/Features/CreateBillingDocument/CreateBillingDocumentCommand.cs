using FluentValidation;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.CreateBillingDocument;

public sealed record CreateBillingDocumentCommand(
    Guid CaseId,
    Guid ClientId,
    BillingDocumentKind Kind,
    BillingMode Mode,
    decimal TaxRatePercent = 21m, // Belgian VAT on lawyers' services (SRD §5)
    string Currency = Money.DefaultCurrency,
    VatRegime Regime = VatRegime.Standard) : ICommand<Result<Guid>>;

public sealed class CreateBillingDocumentValidator : AbstractValidator<CreateBillingDocumentCommand>
{
    public CreateBillingDocumentValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.ClientId).NotEmpty();
        RuleFor(command => command.TaxRatePercent).InclusiveBetween(0m, 100m);
    }
}

public sealed class CreateBillingDocumentCommandHandler(
    IBillingDocumentRepository repository,
    ICaseApi caseApi,
    IClientApi clientApi,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<CreateBillingDocumentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBillingDocumentCommand request, CancellationToken cancellationToken = default)
    {
        if (!await caseApi.CaseExistsAsync(request.CaseId, cancellationToken))
        {
            return Result.Failure<Guid>(BillingErrors.CaseNotFound);
        }

        if (!await clientApi.ClientExistsAsync(request.ClientId, cancellationToken))
        {
            return Result.Failure<Guid>(BillingErrors.ClientNotFound);
        }

        BillingDocument document = BillingDocument.CreateDraft(
            request.CaseId, request.ClientId, request.Kind, request.Mode, request.TaxRatePercent, request.Currency, request.Regime);

        repository.Add(document);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(document.Id.Value);
    }
}

public sealed class CreateBillingDocumentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/documents", async (CreateBillingDocumentCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/billing/documents/{id}", new { id }));
            })
            .WithName("CreateBillingDocument")
            .WithTags("Billing")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }
}
