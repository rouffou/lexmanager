using LexManager.Modules.Calendar.Domain.Common;

namespace LexManager.Modules.Calendar.Application.Abstractions;

/// <summary>Identifier returned by the external calendar after a successful push.</summary>
public sealed record CalendarSyncLink(CalendarProvider Provider, string ExternalId, string? ETag);
