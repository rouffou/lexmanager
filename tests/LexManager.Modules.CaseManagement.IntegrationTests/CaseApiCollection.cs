using LexManager.Modules.CaseManagement.IntegrationTests.Infrastructure;

namespace LexManager.Modules.CaseManagement.IntegrationTests;

[CollectionDefinition(nameof(CaseApiCollection))]
public sealed class CaseApiCollection : ICollectionFixture<CaseApiFactory>;
