using System.Net.Http.Json;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Recommendations;

[Collection("Api")]
public class RecommendationForUserTests : IntegrationTestBase
{
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    public RecommendationForUserTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetForUser_RecommendsOtherMoviesOfLikedGenre_ExcludingLikedAndOtherGenres()
    {
        var userId = Guid.Empty;

        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var drama = TestData.NewGenre("Drama");
            var comedy = TestData.NewGenre("Comedy");

            var liked = TestData.NewMovie(category.Id, "Liked drama");
            liked.Genres.Add(drama);
            var sameGenre = TestData.NewMovie(category.Id, "Another drama");
            sameGenre.Genres.Add(drama);
            var otherGenre = TestData.NewMovie(category.Id, "A comedy");
            otherGenre.Genres.Add(comedy);

            var user = TestData.NewUser();
            userId = user.Id;

            db.Users.Add(user);
            db.Movies.AddRange(liked, sameGenre, otherGenre);
            db.UserActivities.Add(TestData.NewMovieActivity(user.Id, liked.Id, ActionType.Like, 2.0));
            await db.SaveChangesAsync();
        });

        var movies = await Client.GetFromJsonAsync<List<MovieItem>>($"/api/recommendation/user/{userId}");

        Assert.Equal(new[] { "Another drama" }, movies!.Select(m => m.Title));
    }

    [Fact]
    public async Task GetForUser_SortsRecommendationsByReleaseDateDescending()
    {
        var userId = Guid.Empty;

        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var drama = TestData.NewGenre("Drama");

            var liked = TestData.NewMovie(category.Id, "Liked", Today.AddYears(-1));
            var older = TestData.NewMovie(category.Id, "Older", Today.AddYears(-3));
            var newer = TestData.NewMovie(category.Id, "Newer", Today.AddMonths(-2));
            foreach (var m in new[] { liked, older, newer }) m.Genres.Add(drama);

            var user = TestData.NewUser();
            userId = user.Id;

            db.Users.Add(user);
            db.Movies.AddRange(liked, older, newer);
            db.UserActivities.Add(TestData.NewMovieActivity(user.Id, liked.Id, ActionType.Like, 2.0));
            await db.SaveChangesAsync();
        });

        var movies = await Client.GetFromJsonAsync<List<MovieItem>>($"/api/recommendation/user/{userId}");

        Assert.Equal(new[] { "Newer", "Older" }, movies!.Select(m => m.Title));
    }

    [Fact]
    public async Task GetForUser_RecommendsMoviesWithSameActor()
    {
        var userId = Guid.Empty;

        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var actor = TestData.NewActor("Test Actor");
            var otherActor = TestData.NewActor("Someone Else");

            var liked = TestData.NewMovie(category.Id, "Liked");
            liked.Actors.Add(actor);
            var withSameActor = TestData.NewMovie(category.Id, "Same actor");
            withSameActor.Actors.Add(actor);
            var withOtherActor = TestData.NewMovie(category.Id, "Other actor");
            withOtherActor.Actors.Add(otherActor);

            var user = TestData.NewUser();
            userId = user.Id;

            db.Users.Add(user);
            db.Movies.AddRange(liked, withSameActor, withOtherActor);
            db.UserActivities.Add(TestData.NewMovieActivity(user.Id, liked.Id, ActionType.Like, 2.0));
            await db.SaveChangesAsync();
        });

        var movies = await Client.GetFromJsonAsync<List<MovieItem>>($"/api/recommendation/user/{userId}");

        Assert.Equal(new[] { "Same actor" }, movies!.Select(m => m.Title));
    }

    [Fact]
    public async Task GetForUser_UnknownUser_ReturnsEmptyList()
    {
        var movies = await Client.GetFromJsonAsync<List<MovieItem>>($"/api/recommendation/user/{Guid.NewGuid()}");

        Assert.Empty(movies!);
    }
}