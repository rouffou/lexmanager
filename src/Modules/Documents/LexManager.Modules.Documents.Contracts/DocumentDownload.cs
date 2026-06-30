namespace LexManager.Modules.Documents.Contracts;

/// <summary>File payload for download responses (content streamed by the endpoint).</summary>
public sealed record DocumentDownload(string FileName, string ContentType, byte[] Content);
