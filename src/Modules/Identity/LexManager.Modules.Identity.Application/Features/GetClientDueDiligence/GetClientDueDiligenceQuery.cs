using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.GetClientDueDiligence;

public sealed record GetClientDueDiligenceQuery(Guid ClientId) : IQuery<Result<DueDiligenceResponse>>;
