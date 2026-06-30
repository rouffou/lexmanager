using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.CloseCase;

public sealed record CloseCaseCommand(Guid CaseId) : ICommand<Result>;
