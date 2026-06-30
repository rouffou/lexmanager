using LexManager.Modules.Billing.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.GetCarpaAccount;

public sealed record GetCarpaAccountByIdQuery(Guid AccountId) : IQuery<Result<CarpaAccountResponse>>;
