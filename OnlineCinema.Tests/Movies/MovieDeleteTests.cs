using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineCinema.Domain.Models;
using OnlineCinema.Tests.Fakes;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Movies;

[Collection("Api")]
public class MovieDeleteTests : IntegrationTestBase
{
    public MovieDeleteTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Delete_AsRegularUser_ReturnsForbidden()
    {
        var movie = await Factory.SeedMovieAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await user.DeleteAsync($"/api/movie/delete/{movie.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AsAdmin_RemovesMovieAndRequestsFileCleanup()
    {
        var movie = await Factory.SeedMovieAsync();
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.DeleteAsync($"/api/movie/delete/{movie.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var afterDelete = await admin.GetAsync($"/api/movie/{movie.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);

        var fake = Factory.Services.GetRequiredService<FakeMovieUploadService>();
        Assert.Contains(movie.Id, fake.DeletedMovieIds);
    }

    [Fact]
    public async Task Delete_UnknownMovie_ReturnsNotFound()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);
        var id = Guid.NewGuid();

        var response = await admin.DeleteAsync($"/api/movie/delete/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var fake = Factory.Services.GetRequiredService<FakeMovieUploadService>();
        Assert.DoesNotContain(id, fake.DeletedMovieIds);
    }

    [Fact]
    public async Task Delete_AlsoRemovesMovieFavoritesAndComments()
    {
        var movie = await Factory.SeedMovieAsync();
        var (_, userId) = await AuthHelper.CreateUserClientAsync(Factory);
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        await Factory.SeedAsync(async db =>
        {
            db.Favorites.Add(new Favorite { UserId = userId, MovieId = movie.Id });
            db.Comments.Add(new Comment { UserId = userId, MovieId = movie.Id, Content = "Nice" });
            await db.SaveChangesAsync();
        });

        (await admin.DeleteAsync($"/api/movie/delete/{movie.Id}")).EnsureSuccessStatusCode();

        var favorites = await Factory.QueryAsync(db => db.Favorites.CountAsync());
        var comments = await Factory.QueryAsync(db => db.Comments.CountAsync());
        Assert.Equal(0, favorites);
        Assert.Equal(0, comments);
    }
}