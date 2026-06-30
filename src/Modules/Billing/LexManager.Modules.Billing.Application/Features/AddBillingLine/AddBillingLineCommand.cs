using FluentValidation;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.AddBillingLine;

public sealed record AddBillingLineCommand(Guid DocumentId, string Description, decimal Quantity, decimal UnitPrice)
    : ICommand<Result>;

public sealed class AddBillingLineValidator : AbstractValidator<AddBillingLineCommand>
{
    public AddBillingLineValidator()
    {
        RuleFor(command => command.Description).NotEmpty();
        RuleFor(command => command.Quantity).GreaterThan(0m);
        RuleFor(command => command.UnitPrice).GreaterThanOrEqualTo(0m);
    }
}

public sealed class AddBillingLineCommandHandler(
    IBillingDocumentRepository repository,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<AddBillingLineCommand, Result>
{
    public async Task<Result> Handle(AddBillingLineCommand request, CancellationToken cancellationToken = default)
    {
        BillingDocument? document = await repository.GetByIdAsync(new BillingDocumentId(request.DocumentId), cancellationToken);
        if (document is null)
        {
            return Result.Failure(BillingErrors.NotFound);
        }

        document.AddLine(request.Description, request.Quantity, Money.Of(request.UnitPrice, document.Currency));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class AddBillingLineEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/documents/{id:guid}/lines", async (
                Guid id,
                AddBillingLineRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(
                    new AddBillingLineCommand(id, body.Description, body.Quantity, body.UnitPrice), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("AddBillingLine")
            .WithTags("Billing")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

public sealed record AddBillingLineRequest(string Description, decimal Quantity, decimal UnitPrice);
