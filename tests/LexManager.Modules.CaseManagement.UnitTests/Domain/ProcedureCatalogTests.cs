using LexManager.Modules.CaseManagement.Domain.Procedures;

namespace LexManager.Modules.CaseManagement.UnitTests.Domain;

public class ProcedureCatalogTests
{
    [Theory]
    [InlineData(ProcedureType.DebtRecovery)]
    [InlineData(ProcedureType.CivilLitigation)]
    [InlineData(ProcedureType.SummaryProceedings)]
    [InlineData(ProcedureType.LabourDispute)]
    [InlineData(ProcedureType.Appeal)]
    public void For_Should_ReturnContiguousOrderedStages(ProcedureType type)
    {
        IReadOnlyList<ProcedureStageBlueprint> stages = ProcedureCatalog.For(type);

        stages.Should().NotBeEmpty();
        stages.Select(stage => stage.Order).Should().BeInAscendingOrder().And.OnlyHaveUniqueItems();
        stages[0].Order.Should().Be(1);
        stages.Should().OnlyContain(stage => !string.IsNullOrWhiteSpace(stage.Name) && !string.IsNullOrWhiteSpace(stage.Phase));
    }

    [Fact]
    public void For_DebtRecovery_Should_RunFromNoticeToEnforcement()
    {
        IReadOnlyList<ProcedureStageBlueprint> stages = ProcedureCatalog.For(ProcedureType.DebtRecovery);

        stages[0].Name.Should().Be("Mise en demeure");
        stages[^1].Name.Should().Contain("Exécution forcée");
    }

    [Fact]
    public void For_UnknownType_Should_ReturnEmpty()
    {
        ProcedureCatalog.For((ProcedureType)999).Should().BeEmpty();
    }
}
