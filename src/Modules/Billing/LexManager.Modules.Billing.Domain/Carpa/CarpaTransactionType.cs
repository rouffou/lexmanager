namespace LexManager.Modules.Billing.Domain.Carpa;

/// <summary>Direction of a movement on a third-party (rubriqué) account (SRD V11 §5: CARPA).</summary>
public enum CarpaTransactionType
{
    /// <summary>Funds received on behalf of the client (incoming).</summary>
    Deposit = 1,

    /// <summary>Funds paid out of the account (outgoing).</summary>
    Disbursement = 2
}
