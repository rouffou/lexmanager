namespace LexManager.Modules.Documents.Application.Abstractions;

/// <summary>Pointer to a stored document version's bytes plus the metadata needed to serve it.</summary>
public sealed record DocumentFileRef(string FileName, string ContentType, string StorageKey);
