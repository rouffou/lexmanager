using Microsoft.AspNetCore.Routing;

namespace LexManager.Infrastructure.Endpoints;

/// <summary>
/// A single vertical-slice endpoint. Each feature folder owns one of these, keeping the
/// HTTP surface next to its command/query/handler (Vertical Slice Architecture).
/// </summary>
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
