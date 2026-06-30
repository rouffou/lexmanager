using LexManager.Infrastructure.Endpoints;
using LexManager.Modules.Calendar.Application.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.SyncWebhook;

/// <summary>
/// Ingress for external calendar change notifications (Microsoft Graph / Google push, SRD Module 4).
/// Captures modifications made outside the platform; the concrete sync provider decides how to
/// reconcile them. The default provider is a no-op, so this simply acknowledges receipt.
/// </summary>
public sealed class CalendarWebhookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/calendar/webhooks/{provider}", async (
                string provider,
                HttpRequest request,
                ICalendarSyncProvider syncProvider,
                CancellationToken cancellationToken) =>
            {
                using var reader = new StreamReader(request.Body);
                string payload = await reader.ReadToEndAsync(cancellationToken);

                await syncProvider.HandleWebhookAsync(provider, payload, cancellationToken);
                return Results.Accepted();
            })
            .WithName("CalendarWebhook")
            .WithTags("Calendar")
            .DisableAntiforgery()
            .Produces(StatusCodes.Status202Accepted);
    }
}
