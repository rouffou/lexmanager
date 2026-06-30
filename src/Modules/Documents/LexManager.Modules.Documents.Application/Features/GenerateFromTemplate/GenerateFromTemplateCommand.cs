using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Application.Features.GenerateFromTemplate;

/// <summary>Generates a document from a predefined template (publipostage / mail merge, SRD Module 3).</summary>
public sealed record GenerateFromTemplateCommand(
    Guid CaseId,
    string TemplateKey,
    Dictionary<string, string> Values) : ICommand<Result<Guid>>;
