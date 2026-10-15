using OnlineCinema.Tests.Fixtures;

namespace Tests.Fixtures
{
    [CollectionDefinition("Api")]
    public class ApiCollection : ICollectionFixture<CinemaWebApplicationFactory> { }
}
