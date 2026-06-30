using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.GetClientById;

public sealed record GetClientByIdQuery(Guid ClientId) : IQuery<Result<ClientResponse>>;
