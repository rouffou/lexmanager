using LexManager.Modules.Identity.IntegrationTests.Infrastructure;

namespace LexManager.Modules.Identity.IntegrationTests;

[CollectionDefinition(nameof(IdentityApiCollection))]
public sealed class IdentityApiCollection : ICollectionFixture<IdentityApiFactory>;
