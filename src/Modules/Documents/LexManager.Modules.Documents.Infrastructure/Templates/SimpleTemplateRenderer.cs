using System.Text;
using LexManager.Modules.Documents.Application.Abstractions;

namespace LexManager.Modules.Documents.Infrastructure.Templates;

/// <summary>
/// Minimal mail-merge renderer (SRD Module 3: génération de documents). Replaces <c>{{Field}}</c>
/// placeholders in a registered template. A production implementation could load templates from the
/// database and render to PDF/DOCX; the <see cref="ITemplateRenderer"/> port keeps that swappable.
/// </summary>
internal sealed class SimpleTemplateRenderer : ITemplateRenderer
{
    private static readonly Dictionary<string, (string FileName, string Body)> Templates =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["engagement-letter"] = (
                "lettre-de-mission.html",
                "<h1>Lettre de mission</h1><p>Cher/Chère {{ClientName}},</p>" +
                "<p>Objet : {{CaseTitle}}.</p><p>Fait à {{City}}, le {{Date}}.</p>"),
            ["provision-call"] = (
                "appel-de-provision.html",
                "<h1>Appel de provision</h1><p>Dossier : {{CaseTitle}}</p>" +
                "<p>Montant demandé : {{Amount}} EUR</p><p>Échéance : {{DueDate}}</p>")
        };

    public Task<RenderedTemplate?> RenderAsync(
        string templateKey,
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken = default)
    {
        if (!Templates.TryGetValue(templateKey, out (string FileName, string Body) template))
        {
            return Task.FromResult<RenderedTemplate?>(null);
        }

        string body = values.Aggregate(
            template.Body,
            (current, pair) => current.Replace($"{{{{{pair.Key}}}}}", pair.Value));

        byte[] content = Encoding.UTF8.GetBytes(body);
        return Task.FromResult<RenderedTemplate?>(new RenderedTemplate(template.FileName, "text/html; charset=utf-8", content));
    }
}
