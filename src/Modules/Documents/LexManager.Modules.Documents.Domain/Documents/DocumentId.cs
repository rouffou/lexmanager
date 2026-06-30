namespace LexManager.Modules.Documents.Domain.Documents;

/// <summary>Strongly-typed identifier for a <see cref="Document"/>.</summary>
public readonly record struct DocumentId(Guid Value)
{
    public static DocumentId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
