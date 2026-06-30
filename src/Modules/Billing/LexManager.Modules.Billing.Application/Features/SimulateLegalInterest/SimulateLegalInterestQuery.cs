using FluentValidation;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.Domain.Common;
using LexManager.Modules.Billing.Domain.Interest;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.SimulateLegalInterest;

/// <summary>Simulates Belgian legal default interest on a condemnation (SRD V11 §5).</summary>
public sealed record SimulateLegalInterestQuery(
    decimal Principal,
    DateOnly FromDate,
    DateOnly ToDate,
    decimal AnnualRatePercent,
    bool Capitalize = false,
    string Currency = Money.DefaultCurrency) : IQuery<Result<LegalInterestResponse>>;

public sealed class SimulateLegalInterestValidator : AbstractValidator<SimulateLegalInterestQuery>
{
    public SimulateLegalInterestValidator()
    {
        RuleFor(query => query.Principal).GreaterThan(0m);
        RuleFor(query => query.AnnualRatePercent).GreaterThan(0m);
        RuleFor(query => query.ToDate).GreaterThan(query => query.FromDate);
    }
}

public sealed class SimulateLegalInterestQueryHandler
    : IQueryHandler<SimulateLegalInterestQuery, Result<LegalInterestResponse>>
{
    public Task<Result<LegalInterestResponse>> Handle(SimulateLegalInterestQuery request, CancellationToken cancellationToken = default)
    {
        LegalInterestResult result = BelgianLegalInterestCalculator.Compute(
            Money.Of(request.Principal, request.Currency),
            request.FromDate,
            request.ToDate,
            request.AnnualRatePercent,
            request.Capitalize);

        var response = new LegalInterestResponse(
            result.Principal.Amount,
            result.Principal.Currency,
            result.FromDate,
            result.ToDate,
            result.Days,
            result.AnnualRatePercent,
            result.Capitalized,
            result.Interest.Amount,
            result.Total.Amount);

        return Task.FromResult(Result.Success(response));
    }
}

public sealed class SimulateLegalInterestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/legal-interest/simulate", async (
                SimulateLegalInterestQuery query,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<LegalInterestResponse> result = await sender.Send(query, cancellationToken);
                return result.ToApiResult();
            })
            .WithName("SimulateLegalInterest")
            .WithTags("Billing")
            .Produces<LegalInterestResponse>()
            .ProducesValidationProblem();
    }
}
