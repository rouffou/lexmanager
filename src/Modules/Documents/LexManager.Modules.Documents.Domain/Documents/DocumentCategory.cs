namespace LexManager.Modules.Documents.Domain.Documents;

/// <summary>Classification of a stored document (SRD Module 3).</summary>
public enum DocumentCategory
{
    ProcedureDocument = 1,
    Conclusions = 2,
    Contract = 3,
    Correspondence = 4,
    Generated = 5,
    Other = 6
}
