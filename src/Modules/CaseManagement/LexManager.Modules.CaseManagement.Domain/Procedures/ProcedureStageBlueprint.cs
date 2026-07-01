namespace LexManager.Modules.CaseManagement.Domain.Procedures;

/// <summary>
/// A template milestone in a procedure's blueprint: its rank, label and the judicial phase it
/// belongs to (used to group the tree, e.g. Amiable, Introduction, Instruction, Jugement, Exécution).
/// </summary>
public sealed record ProcedureStageBlueprint(int Order, string Name, string Phase);
