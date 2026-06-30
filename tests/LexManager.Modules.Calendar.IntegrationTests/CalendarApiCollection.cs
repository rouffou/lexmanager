using LexManager.Modules.Calendar.IntegrationTests.Infrastructure;

namespace LexManager.Modules.Calendar.IntegrationTests;

[CollectionDefinition(nameof(CalendarApiCollection))]
public sealed class CalendarApiCollection : ICollectionFixture<CalendarApiFactory>;
