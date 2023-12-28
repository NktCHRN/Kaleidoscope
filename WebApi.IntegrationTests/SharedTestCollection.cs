using Xunit;

namespace WebApi.IntegrationTests;
[CollectionDefinition("Test collection", DisableParallelization = true)]
public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}
