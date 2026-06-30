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

namespace LexManager.Modules.Billing.Application.Features.IssueBillingDocument;

public sealed record IssueBillingDocumentCommand(Guid DocumentId, DateTime DueDateUtc) : ICommand<Result<string>>;

public sealed class IssueBillingDocumentCommandHandler(
    IBillingDocumentRepository repository,
    IInvoiceNumberGenerator numberGenerator,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<IssueBillingDocumentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(IssueBillingDocumentCommand request, CancellationToken cancellationToken = default)
    {
        BillingDocument? document = await repository.GetByIdAsync(new BillingDocumentId(request.DocumentId), cancellationToken);
        if (document is null)
        {
            return Result.Failure<string>(BillingErrors.NotFound);
        }

        if (document.Status != BillingStatus.Draft)
        {
            return Result.Failure<string>(BillingErrors.NotEditable);
        }

        if (document.Lines.Count == 0)
        {
            return Result.Failure<string>(BillingErrors.CannotIssueEmpty);
        }

        string number = await numberGenerator.NextAsync(document.Kind, cancellationToken);
        document.Issue(number, request.DueDateUtc);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(number);
    }
}

public sealed class IssueBillingDocumentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/documents/{id:guid}/issue", async (
                Guid id,
                IssueBillingDocumentRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<string> result = await sender.Send(new IssueBillingDocumentCommand(id, body.DueDateUtc), cancellationToken);
                return result.ToApiResult(number => Results.Ok(new { number }));
            })
            .WithName("IssueBillingDocument")
            .WithTags("Billing")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

public sealed record IssueBillingDocumentRequest(DateTime DueDateUtc);
