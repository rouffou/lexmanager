using LexManager.Modules.Identity.Domain.Compliance;

namespace LexManager.Modules.Identity.Application.Features.RecordVerificationCheck;

public sealed record RecordVerificationCheckRequest(VerificationKind Kind, string Reference, bool Cleared, string? Notes);
