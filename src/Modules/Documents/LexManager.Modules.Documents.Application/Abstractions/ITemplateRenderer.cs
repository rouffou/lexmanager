namespace LexManager.Modules.Documents.Application.Abstractions;

/// <summary>Renders a named template against a set of field values into a document.</summary>
public interface ITemplateRenderer
{
    Task<RenderedTemplate?> RenderAsync(
        string templateKey,
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken = default);
}
