namespace UrlShortener.WebApi.Tests.Fixtures;

[CollectionDefinition(nameof(IntegrationTestsCollection))]
public class IntegrationTestsCollection : ICollectionFixture<PosgreSqlContainerFixture>
{
}
