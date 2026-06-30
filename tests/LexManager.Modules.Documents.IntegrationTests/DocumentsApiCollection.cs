using LexManager.Modules.Documents.IntegrationTests.Infrastructure;

namespace LexManager.Modules.Documents.IntegrationTests;

[CollectionDefinition(nameof(DocumentsApiCollection))]
public sealed class DocumentsApiCollection : ICollectionFixture<DocumentsApiFactory>;
