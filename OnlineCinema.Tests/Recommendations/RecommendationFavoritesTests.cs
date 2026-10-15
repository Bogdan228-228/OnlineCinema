using System.Net.Http.Json;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Recommendations;

[Collection("Api")]
public class RecommendationFavoritesTests : IntegrationTestBase
{
    public RecommendationFavoritesTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetPopular_CountsMoviesAddedToFavorites()
    {
        var favoritedId = Guid.Empty;

        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var favorited = TestData.NewMovie(category.Id, "Favorited");
            var ignored = TestData.NewMovie(category.Id, "Ignored");
            db.Movies.AddRange(favorited, ignored);
            favoritedId = favorited.Id;
            await db.SaveChangesAsync();
        });

        var (userClient, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var add = await userClient.PostAsJsonAsync("/api/favorite", new { contentId = favoritedId });
        add.EnsureSuccessStatusCode();

        var movies = await Client.GetFromJsonAsync<List<MovieItem>>("/api/recommendation/popular");

        Assert.Equal(new[] { "Favorited" }, movies!.Select(m => m.Title));
    }

    [Fact]
    public async Task GetPopular_AfterRemovingFromFavorites_NoLongerCountsMovie()
    {
        var movieId = Guid.Empty;

        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var movie = TestData.NewMovie(category.Id, "Temporarily favorited");
            db.Movies.Add(movie);
            movieId = movie.Id;
            await db.SaveChangesAsync();
        });

        var (userClient, _) = await AuthHelper.CreateUserClientAsync(Factory);

        (await userClient.PostAsJsonAsync("/api/favorite", new { contentId = movieId }))
            .EnsureSuccessStatusCode();
        (await userClient.DeleteAsync($"/api/favorite/{movieId}"))
            .EnsureSuccessStatusCode();

        var movies = await Client.GetFromJsonAsync<List<MovieItem>>("/api/recommendation/popular");

        Assert.Empty(movies!);
    }
}