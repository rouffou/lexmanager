using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Billing;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.ProcessOverdue;

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
