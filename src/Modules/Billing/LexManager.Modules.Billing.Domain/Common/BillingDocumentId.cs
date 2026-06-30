namespace LexManager.Modules.Billing.Domain.Common;

public readonly record struct BillingDocumentId(Guid Value)
{
    public static BillingDocumentId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
