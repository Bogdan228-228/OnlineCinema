using OnlineCinema.Tests.Fixtures;

namespace OnlineCinema.Tests.Helpers
{
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected readonly CinemaWebApplicationFactory Factory;
        protected readonly HttpClient Client;

        protected IntegrationTestBase(CinemaWebApplicationFactory factory)
        {
            Factory = factory;
            Client = factory.CreateApiClient();
        }

        public Task InitializeAsync() => Factory.ResetDatabaseAsync();
        public Task DisposeAsync() => Task.CompletedTask;
    }
}
