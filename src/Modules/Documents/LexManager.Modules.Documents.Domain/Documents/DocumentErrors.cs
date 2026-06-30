using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Documents.Domain.Documents;

public static class DocumentErrors
{
    public static readonly ResultError NotFound = ResultError.NotFound(
        "Document.NotFound", "No document was found for the supplied identifier.");

    public static readonly ResultError CaseNotFound = ResultError.Problem(
        "Document.CaseNotFound", "The case to attach the document to does not exist.");

    public static readonly ResultError EmptyContent = ResultError.Failure(
        "Document.EmptyContent", "The uploaded document is empty.");

    public static readonly ResultError MissingFileName = ResultError.Failure(
        "Document.MissingFileName", "A document requires a file name.");

    public static readonly ResultError TemplateNotFound = ResultError.NotFound(
        "Document.TemplateNotFound", "The requested document template does not exist.");
}
