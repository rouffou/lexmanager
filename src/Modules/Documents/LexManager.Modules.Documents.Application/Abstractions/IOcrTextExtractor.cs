namespace LexManager.Modules.Documents.Application.Abstractions;

/// <summary>
/// Extracts searchable plain text from a document's binary content (SRD §7.2). Scanned images are
/// run through OCR (Tesseract, FR + NL); already-textual formats are decoded directly. The result
/// feeds the full-text search index. Confidential content is processed in-process and never leaves
/// the deployment (SRD §3, Module 3).
/// </summary>
public interface IOcrTextExtractor
{
    /// <summary>Whether this extractor can produce text for the given MIME type.</summary>
    bool CanExtract(string contentType);

    /// <summary>
    /// Returns the extracted text, or an empty string when the content is unsupported or the
    /// extraction fails (extraction is best-effort and never blocks an upload).
    /// </summary>
    Task<string> ExtractTextAsync(byte[] content, string contentType, CancellationToken cancellationToken = default);
}
