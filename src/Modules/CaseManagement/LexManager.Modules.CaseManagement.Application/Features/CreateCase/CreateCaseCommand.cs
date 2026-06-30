using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.CreateCase;

public sealed record CreateCaseCommand(
    string Title,
    Guid ClientId,
    string? CourtName = null,
    string? GeneralRegisterNumber = null,
    string? Judge = null) : ICommand<Result<Guid>>;
