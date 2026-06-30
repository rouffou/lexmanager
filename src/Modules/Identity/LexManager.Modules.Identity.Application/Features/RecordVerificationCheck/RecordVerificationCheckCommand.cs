using LexManager.Modules.Identity.Domain.Compliance;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.RecordVerificationCheck;

public sealed record RecordVerificationCheckCommand(
    Guid DueDiligenceId,
    VerificationKind Kind,
    string Reference,
    bool Cleared,
    string? Notes) : ICommand<Result>;
