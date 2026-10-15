using System.Net.Http.Json;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Recommendations
{
    [Collection("Api")]
    public class RecommendationNewReleasesTests : IntegrationTestBase
    {
        private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

        public RecommendationNewReleasesTests(CinemaWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task GetNew_ByDefault_ReturnsOnlyMoviesFromLastSixMonths()
        {
            await SeedMoviesAsync(
                ("Old", Today.AddYears(-3)),
                ("Recent", Today.AddMonths(-1)));

            var movies = await Client.GetFromJsonAsync<List<MovieItem>>("/api/recommendation/new");

            Assert.Equal(new[] { "Recent" }, movies!.Select(m => m.Title));
        }

        [Fact]
        public async Task GetNew_SortsByReleaseDateDescending()
        {
            await SeedMoviesAsync(
                ("Middle", Today.AddMonths(-3)),
                ("Newest", Today.AddDays(-5)),
                ("Oldest", Today.AddMonths(-5)));

            var movies = await Client.GetFromJsonAsync<List<MovieItem>>("/api/recommendation/new");

            Assert.Equal(new[] { "Newest", "Middle", "Oldest" }, movies!.Select(m => m.Title));
        }

        [Fact]
        public async Task GetNew_WithLargerMonthsParameter_IncludesOlderMovies()
        {
            await SeedMoviesAsync(("EightMonthsAgo", Today.AddMonths(-8)));

            var byDefault = await Client.GetFromJsonAsync<List<MovieItem>>("/api/recommendation/new");
            var withTwelve = await Client.GetFromJsonAsync<List<MovieItem>>("/api/recommendation/new?months=12");

            Assert.Empty(byDefault!);
            Assert.Single(withTwelve!);
        }

        private Task SeedMoviesAsync(params (string Title, DateOnly Released)[] movies)
            => Factory.SeedAsync(async db =>
            {
                var category = TestData.NewCategory();
                db.Categories.Add(category);
                await db.SaveChangesAsync();

                db.Movies.AddRange(movies.Select(m => TestData.NewMovie(category.Id, m.Title, m.Released)));
                await db.SaveChangesAsync();
            });
    }
}
