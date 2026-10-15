using System.Net;
using System.Net.Http.Json;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Statistics;

[Collection("Api")]
public class UserStatsTests : IntegrationTestBase
{
    public UserStatsTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetStats_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/users/stats");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetStats_ForNewUser_ReturnsZeros()
    {
        var (client, userId) = await AuthHelper.CreateUserClientAsync(Factory);

        var stats = await client.GetFromJsonAsync<UserStatsItem>("/api/users/stats");

        Assert.Equal(userId, stats!.UserId);
        Assert.Equal(0, stats.TotalViews);
        Assert.Equal(0, stats.TotalLikes);
        Assert.Equal(0, stats.TotalComments);
        Assert.Equal(0, stats.TotalFavorites);
        Assert.Equal(0, stats.CurrentStreak);
        Assert.Empty(stats.RecentlyViewed);
        Assert.Empty(stats.TopGenres);
    }

    [Fact]
    public async Task GetStats_CountsViewsLikesDislikesCommentsAndFavorites()
    {
        var (client, userId) = await AuthHelper.CreateUserClientAsync(Factory);

        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var movieA = TestData.NewMovie(category.Id, "Movie A");
            var movieB = TestData.NewMovie(category.Id, "Movie B");
            db.Movies.AddRange(movieA, movieB);

            db.UserActivities.AddRange(
                TestData.NewMovieActivity(userId, movieA.Id, ActionType.View),
                TestData.NewMovieActivity(userId, movieA.Id, ActionType.View),
                TestData.NewMovieActivity(userId, movieB.Id, ActionType.View),
                TestData.NewMovieActivity(userId, movieA.Id, ActionType.Like, 2.0),
                TestData.NewMovieActivity(userId, movieB.Id, ActionType.Dislike, -1.0));

            db.Comments.Add(new Comment { UserId = userId, MovieId = movieA.Id, Content = "Nice" });
            db.Favorites.Add(new Favorite { UserId = userId, MovieId = movieB.Id });

            await db.SaveChangesAsync();
        });

        var stats = await client.GetFromJsonAsync<UserStatsItem>("/api/users/stats");

        Assert.Equal(3, stats!.TotalViews);
        Assert.Equal(1, stats.TotalLikes);
        Assert.Equal(1, stats.TotalDislikes);
        Assert.Equal(1, stats.TotalComments);
        Assert.Equal(1, stats.TotalFavorites);
        Assert.Equal(3, stats.MoviesThisWeek);
        Assert.Equal(3, stats.MoviesThisMonth);
        Assert.Equal(1, stats.CurrentStreak);
        Assert.Equal(2, stats.RecentlyViewed.Count);
    }

    [Fact]
    public async Task GetStats_TopGenres_AreOrderedByNumberOfViewedMovies()
    {
        var (client, userId) = await AuthHelper.CreateUserClientAsync(Factory);

        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var drama = TestData.NewGenre("Drama");
            var comedy = TestData.NewGenre("Comedy");

            var drama1 = TestData.NewMovie(category.Id, "Drama 1");
            var drama2 = TestData.NewMovie(category.Id, "Drama 2");
            var comedy1 = TestData.NewMovie(category.Id, "Comedy 1");
            drama1.Genres.Add(drama);
            drama2.Genres.Add(drama);
            comedy1.Genres.Add(comedy);

            db.Movies.AddRange(drama1, drama2, comedy1);
            db.UserActivities.AddRange(
                TestData.NewMovieActivity(userId, drama1.Id, ActionType.View),
                TestData.NewMovieActivity(userId, drama2.Id, ActionType.View),
                TestData.NewMovieActivity(userId, comedy1.Id, ActionType.View));

            await db.SaveChangesAsync();
        });

        var stats = await client.GetFromJsonAsync<UserStatsItem>("/api/users/stats");

        Assert.Equal("Drama", stats!.TopGenres[0].Name);
        Assert.Equal(2, stats.TopGenres[0].Count);
        Assert.Equal("Comedy", stats.TopGenres[1].Name);
    }
}