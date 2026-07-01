using System.Diagnostics;
using System.Text;
using LexManager.Modules.Documents.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace LexManager.Modules.Documents.Infrastructure.Ocr;

/// <summary>
/// Text extraction backed by the Tesseract CLI (FR + NL). Already-textual formats are decoded
/// in-process (no OCR needed); images are rasterised and OCR'd by tesseract. Extraction is
/// best-effort: any failure — including a missing binary on a dev machine — yields an empty string
/// so an upload is never blocked, only indexed by file name (SRD §7.2).
/// </summary>
internal sealed class TesseractTextExtractor(TesseractOptions options, ILogger<TesseractTextExtractor> logger)
    : IOcrTextExtractor
{
    private static readonly HashSet<string> TextTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "text/plain", "text/html", "text/csv", "text/markdown", "application/json", "application/xml", "text/xml",
    };

    private static readonly HashSet<string> ImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png", "image/jpeg", "image/jpg", "image/tiff", "image/bmp", "image/gif", "image/webp",
    };

    public bool CanExtract(string contentType)
    {
        string type = Normalize(contentType);
        return TextTypes.Contains(type) || ImageTypes.Contains(type);
    }

    public async Task<string> ExtractTextAsync(byte[] content, string contentType, CancellationToken cancellationToken = default)
    {
        if (content is null || content.Length == 0)
        {
            return string.Empty;
        }

        string type = Normalize(contentType);

        if (TextTypes.Contains(type))
        {
            return DecodeText(content);
        }

        if (ImageTypes.Contains(type))
        {
            return await RunTesseractAsync(content, cancellationToken);
        }

        return string.Empty;
    }

    private static string Normalize(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return string.Empty;
        }

        int separator = contentType.IndexOf(';');
        string type = separator >= 0 ? contentType[..separator] : contentType;
        return type.Trim().ToLowerInvariant();
    }

    private static string DecodeText(byte[] content)
    {
        // Strip a UTF-8 BOM if present, then decode.
        if (content.Length >= 3 && content[0] == 0xEF && content[1] == 0xBB && content[2] == 0xBF)
        {
            return Encoding.UTF8.GetString(content, 3, content.Length - 3).Trim();
        }

        return Encoding.UTF8.GetString(content).Trim();
    }

    private async Task<string> RunTesseractAsync(byte[] content, CancellationToken cancellationToken)
    {
        string inputPath = Path.Combine(Path.GetTempPath(), $"lex-ocr-{Guid.NewGuid():N}");
        try
        {
            await File.WriteAllBytesAsync(inputPath, content, cancellationToken);

            var startInfo = new ProcessStartInfo
            {
                FileName = options.ExecutablePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            // tesseract <image> stdout -l fra+nld
            startInfo.ArgumentList.Add(inputPath);
            startInfo.ArgumentList.Add("stdout");
            startInfo.ArgumentList.Add("-l");
            startInfo.ArgumentList.Add(options.Languages);

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            using var timeout = new CancellationTokenSource(options.Timeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);

            Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync(linked.Token);
            Task<string> stderrTask = process.StandardError.ReadToEndAsync(linked.Token);

            await process.WaitForExitAsync(linked.Token);
            await Task.WhenAll(stdoutTask, stderrTask);

            return (await stdoutTask).Trim();
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "OCR extraction failed; the document will be indexed by file name only.");
            return string.Empty;
        }
        finally
        {
            TryDelete(inputPath);
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
            // Best-effort cleanup of the temp OCR input.
        }
    }
}
