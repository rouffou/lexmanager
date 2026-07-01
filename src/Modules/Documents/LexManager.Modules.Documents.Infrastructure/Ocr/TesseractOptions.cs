namespace LexManager.Modules.Documents.Infrastructure.Ocr;

/// <summary>
/// Configuration for the Tesseract OCR extractor. The runtime image ships the <c>tesseract</c>
/// binary with the French and Dutch language packs (see Dockerfile); these can be overridden via
/// the <c>Ocr:*</c> configuration section for other environments.
/// </summary>
public sealed class TesseractOptions
{
    /// <summary>Path to the tesseract executable (looked up on PATH by default).</summary>
    public string ExecutablePath { get; init; } = "tesseract";

    /// <summary>Language packs passed to <c>-l</c>. Belgium ⇒ French + Dutch (SRD §7.2).</summary>
    public string Languages { get; init; } = "fra+nld";

    /// <summary>How long to wait for a single OCR run before giving up.</summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
}
