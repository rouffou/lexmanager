using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Billing;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.ProcessOverdue;

/// <summary>
/// Flags issued documents whose due date has passed as overdue and fires a payment reminder
/// (SRD Module 5: relances automatiques). Invoked on demand here; a background worker will call
/// it on a schedule (cross-cutting concern).
/// </summary>
public sealed record ProcessOverdueCommand : ICommand<Result<int>>;

public sealed class ProcessOverdueCommandHandler(
    IBillingDocumentRepository repository,
    IPaymentReminderSender reminderSender,
    IBillingUnitOfWork unitOfWork) : ICommandHandler<ProcessOverdueCommand, Result<int>>
{
    public async Task<Result<int>> Handle(ProcessOverdueCommand request, CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;
        IReadOnlyList<BillingDocument> candidates = await repository.GetOverdueCandidatesAsync(now, cancellationToken);

        int reminded = 0;
        foreach (BillingDocument document in candidates)
        {
            if (document.MarkOverdueIfDue(now))
            {
                await reminderSender.SendAsync(document.Id.Value, document.ClientId, document.Total.Amount, cancellationToken);
                reminded++;
            }
        }

        if (reminded > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(reminded);
    }
}

public sealed class ProcessOverdueEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/process-overdue", async (ISender sender, CancellationToken cancellationToken) =>
            {
                Result<int> result = await sender.Send(new ProcessOverdueCommand(), cancellationToken);
                return result.ToApiResult(count => Results.Ok(new { remindersSent = count }));
            })
            .WithName("ProcessOverdueBilling")
            .WithTags("Billing")
            .Produces(StatusCodes.Status200OK);
    }
}
