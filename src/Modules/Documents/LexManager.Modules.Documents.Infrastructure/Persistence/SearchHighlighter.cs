namespace LexManager.Modules.Documents.Infrastructure.Persistence;

/// <summary>
/// Builds a short, human-readable excerpt of an OCR-extracted body around the first matching search
/// word — the "headline" shown next to a full-text search hit (SRD §7.2). Kept in-process (rather
/// than using PostgreSQL <c>ts_headline</c>) so it works uniformly for every result on the page.
/// </summary>
internal static class SearchHighlighter
{
    private const int WindowRadius = 60;
    private const int MaxLength = 160;
    private static readonly char[] Separators = [' ', '\t', '\r', '\n', '"', '-'];

    public static string? BuildSnippet(string? text, string term)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        string normalized = System.Text.RegularExpressions.Regex.Replace(text.Trim(), @"\s+", " ");

        string? firstWord = term
            .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(word => word.Length > 1);

        int match = firstWord is null
            ? -1
            : normalized.IndexOf(firstWord, StringComparison.OrdinalIgnoreCase);

        if (match < 0)
        {
            // No term hit (e.g. the match was on the file name) — return a leading excerpt.
            return Truncate(normalized, 0, MaxLength);
        }

        int start = Math.Max(0, match - WindowRadius);
        string snippet = Truncate(normalized, start, MaxLength);
        return start > 0 ? "… " + snippet : snippet;
    }

    private static string Truncate(string text, int start, int length)
    {
        if (start >= text.Length)
        {
            return string.Empty;
        }

        int available = text.Length - start;
        if (available <= length)
        {
            return text.Substring(start);
        }

        return text.Substring(start, length).TrimEnd() + " …";
    }
}
