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

namespace LexManager.Modules.Billing.Application.Features.RegisterPayment;

public sealed record RegisterPaymentCommand(Guid DocumentId) : ICommand<Result>;

public sealed class RegisterPaymentCommandHandler(
    IBillingDocumentRepository repository,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<RegisterPaymentCommand, Result>
{
    public async Task<Result> Handle(RegisterPaymentCommand request, CancellationToken cancellationToken = default)
    {
        BillingDocument? document = await repository.GetByIdAsync(new BillingDocumentId(request.DocumentId), cancellationToken);
        if (document is null)
        {
            return Result.Failure(BillingErrors.NotFound);
        }

        if (document.Status is not (BillingStatus.Issued or BillingStatus.Overdue))
        {
            return Result.Failure(BillingErrors.NotIssued);
        }

        document.RegisterPayment();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class RegisterPaymentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/documents/{id:guid}/payments", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(new RegisterPaymentCommand(id), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("RegisterPayment")
            .WithTags("Billing")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
