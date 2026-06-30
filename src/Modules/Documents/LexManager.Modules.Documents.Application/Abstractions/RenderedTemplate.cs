namespace LexManager.Modules.Documents.Application.Abstractions;

/// <summary>Result of rendering a document template (mail merge / publipostage, SRD Module 3).</summary>
public sealed record RenderedTemplate(string FileName, string ContentType, byte[] Content);
