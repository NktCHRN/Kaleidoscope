using Xunit;

namespace WebApi.IntegrationTests;
[CollectionDefinition("Test collection")]
public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{
}
