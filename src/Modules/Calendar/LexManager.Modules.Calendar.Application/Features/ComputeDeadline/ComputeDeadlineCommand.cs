using LexManager.Modules.Calendar.Domain.Common;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.ComputeDeadline;

/// <summary>Computes a legal deadline and, optionally, places it on the agenda (SRD Module 4).</summary>
public sealed record ComputeDeadlineCommand(
    DateOnly BaseDate,
    LegalDeadlineType Type,
    Guid? OwnerUserId = null,
    Guid? CaseId = null,
    bool Schedule = false) : ICommand<Result<ComputeDeadlineResponse>>;
