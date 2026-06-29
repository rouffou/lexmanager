namespace LexManager.Application.Abstractions.Messaging;

/// <summary>
/// Marker for a module's public, cross-module API surface. Modules communicate ONLY through
/// these contracts (or via in-process integration events) — never through each other's
/// database or internal types (SRD §3.2). Architecture tests enforce this boundary.
/// </summary>
public interface IModuleApi;
