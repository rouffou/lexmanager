using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Infrastructure.UnitTests.Audit;

public sealed class PingHandler : ICommandHandler<PingCommand, Result<string>>
{
    public Task<Result<string>> Handle(PingCommand request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success(request.Message));
}
