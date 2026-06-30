using LexManager.Modules.Billing.IntegrationTests.Infrastructure;

namespace LexManager.Modules.Billing.IntegrationTests;

[CollectionDefinition(nameof(BillingApiCollection))]
public sealed class BillingApiCollection : ICollectionFixture<BillingApiFactory>;
