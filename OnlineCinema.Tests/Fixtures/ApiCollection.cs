using Xunit;

namespace OnlineCinema.Tests.Fixtures
{
    [CollectionDefinition("Api")]
    public class ApiCollection : ICollectionFixture<CinemaWebApplicationFactory> { }
}
