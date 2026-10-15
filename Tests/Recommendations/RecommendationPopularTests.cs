using System.Net.Http.Json;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;

namespace OnlineCinema.Tests.Recommendations;

[Collection("Api")]
public class RecommendationPopularTests : IntegrationTestBase
{
    public RecommendationPopularTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetPopular_OrdersMoviesByNumberOfLikes()
    {
        await SeedMoviesWithActivitiesAsync(
            ("Two likes", ActionType.Like, 2),
            ("Three likes", ActionType.Like, 3),
            ("One like", ActionType.Like, 1));

        var movies = await Client.GetFromJsonAsync<List<MovieItem>>("/api/recommendation/popular");

        Assert.Equal(new[] { "Three likes", "Two likes", "One like" }, movies!.Select(m => m.Title));
    }

    [Fact]
    public async Task GetPopular_RespectsCountParameter()
    {
        await SeedMoviesWithActivitiesAsync(
            ("Two likes", ActionType.Like, 2),
            ("Three likes", ActionType.Like, 3),
            ("One like", ActionType.Like, 1));

        var movies = await Client.GetFromJsonAsync<List<MovieItem>>("/api/recommendation/popular?count=2");

        Assert.Equal(new[] { "Three likes", "Two likes" }, movies!.Select(m => m.Title));
    }

    [Fact]
    public async Task GetPopular_IgnoresMoviesThatOnlyHaveViews()
    {
        await SeedMoviesWithActivitiesAsync(
            ("Only views", ActionType.View, 3),
            ("Liked", ActionType.Like, 1));

        var movies = await Client.GetFromJsonAsync<List<MovieItem>>("/api/recommendation/popular");

        Assert.Equal(new[] { "Liked" }, movies!.Select(m => m.Title));
    }

    [Fact]
    public async Task GetPopular_WhenNoActivity_ReturnsEmptyList()
    {
        var movies = await Client.GetFromJsonAsync<List<MovieItem>>("/api/recommendation/popular");

        Assert.Empty(movies!);
    }

    private Task SeedMoviesWithActivitiesAsync(params (string Title, ActionType Action, int Count)[] movies)
        => Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var users = Enumerable.Range(0, movies.Max(m => m.Count))
                .Select(_ => TestData.NewUser())
                .ToList();
            db.Users.AddRange(users);

            foreach (var (title, action, count) in movies)
            {
                var movie = TestData.NewMovie(category.Id, title);
                db.Movies.Add(movie);
                db.UserActivities.AddRange(
                    users.Take(count).Select(u => TestData.NewMovieActivity(u.Id, movie.Id, action)));
            }

            await db.SaveChangesAsync();
        });
}