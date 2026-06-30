using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Infrastructure.UnitTests.Audit;

public sealed record PingCommand(string Message) : ICommand<Result<string>>;
