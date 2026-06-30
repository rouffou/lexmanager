using FluentValidation;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using LexManager.Modules.Calendar.Contracts;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.GenerateTimeBasedInvoice;

/// <summary>
/// Builds and issues a time-based invoice for a case by pulling billable time from the Calendar
/// module's <see cref="ITimeTrackingApi"/> and applying an hourly rate (SRD Module 5: au temps passé).
/// </summary>
public sealed record GenerateTimeBasedInvoiceCommand(
    Guid CaseId,
    Guid ClientId,
    decimal HourlyRate,
    DateTime DueDateUtc,
    decimal TaxRatePercent = 20m) : ICommand<Result<Guid>>;

public sealed class GenerateTimeBasedInvoiceValidator : AbstractValidator<GenerateTimeBasedInvoiceCommand>
{
    public GenerateTimeBasedInvoiceValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.ClientId).NotEmpty();
        RuleFor(command => command.HourlyRate).GreaterThan(0m);
    }
}

public sealed class GenerateTimeBasedInvoiceCommandHandler(
    IBillingDocumentRepository repository,
    ICaseApi caseApi,
    IClientApi clientApi,
    ITimeTrackingApi timeTrackingApi,
    IInvoiceNumberGenerator numberGenerator,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<GenerateTimeBasedInvoiceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(GenerateTimeBasedInvoiceCommand request, CancellationToken cancellationToken = default)
    {
        if (!await caseApi.CaseExistsAsync(request.CaseId, cancellationToken))
        {
            return Result.Failure<Guid>(BillingErrors.CaseNotFound);
        }

        if (!await clientApi.ClientExistsAsync(request.ClientId, cancellationToken))
        {
            return Result.Failure<Guid>(BillingErrors.ClientNotFound);
        }

        CaseTimeSummaryResponse time = await timeTrackingApi.GetCaseTimeSummaryAsync(request.CaseId, cancellationToken);
        if (time.BillableMinutes <= 0)
        {
            return Result.Failure<Guid>(BillingErrors.NoBillableTime);
        }

        decimal hours = decimal.Round(time.BillableMinutes / 60m, 2, MidpointRounding.AwayFromZero);

        BillingDocument document = BillingDocument.CreateDraft(
            request.CaseId, request.ClientId, BillingDocumentKind.FeeNote, BillingMode.TimeBased, request.TaxRatePercent);

        document.AddLine(
            $"Honoraires au temps passé ({time.BillableMinutes} min)",
            hours,
            Money.Of(request.HourlyRate));

        string number = await numberGenerator.NextAsync(BillingDocumentKind.FeeNote, cancellationToken);
        document.Issue(number, request.DueDateUtc);

        repository.Add(document);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(document.Id.Value);
    }
}

public sealed class GenerateTimeBasedInvoiceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/documents/time-based", async (
                GenerateTimeBasedInvoiceCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/billing/documents/{id}", new { id }));
            })
            .WithName("GenerateTimeBasedInvoice")
            .WithTags("Billing")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
