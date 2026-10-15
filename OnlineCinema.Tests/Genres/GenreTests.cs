using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Genres;

[Collection("Api")]
public class GenreTests : IntegrationTestBase
{
    public GenreTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Theory]
    [InlineData("POST", "/api/genre/add")]
    [InlineData("PUT", "/api/genre/edit/1")]
    [InlineData("DELETE", "/api/genre/delete/1")]
    public async Task AdminEndpoints_WithoutToken_ReturnUnauthorized(string method, string url)
    {
        var response = await Client.SendAsync(new HttpRequestMessage(new HttpMethod(method), url));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("POST", "/api/genre/add")]
    [InlineData("PUT", "/api/genre/edit/1")]
    [InlineData("DELETE", "/api/genre/delete/1")]
    public async Task AdminEndpoints_AsRegularUser_ReturnForbidden(string method, string url)
    {
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await user.SendAsync(new HttpRequestMessage(new HttpMethod(method), url));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Add_AsAdmin_ReturnsCreatedGenre()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PostAsJsonAsync("/api/genre/add", new { name = "Thriller" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var genre = await response.Content.ReadFromJsonAsync<GenreItem>();
        Assert.Equal("Thriller", genre!.Name);
        Assert.True(genre.Id > 0);
    }

    [Fact]
    public async Task Edit_AsAdmin_RenamesGenre()
    {
        var id = await SeedGenreAsync("Old name");
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PutAsJsonAsync($"/api/genre/edit/{id}", new { id, name = "Renamed" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var genre = await response.Content.ReadFromJsonAsync<GenreItem>();
        Assert.Equal("Renamed", genre!.Name);
    }

    [Fact]
    public async Task Delete_AsAdmin_RemovesGenre()
    {
        var id = await SeedGenreAsync();
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.DeleteAsync($"/api/genre/delete/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var afterDelete = await admin.GetAsync($"/api/genre/{id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownGenre_ReturnsNotFoundWithGenreMessage()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.DeleteAsync("/api/genre/delete/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<MessageItem>();
        Assert.Contains("Genre", body!.Message);
    }

    [Fact]
    public async Task Delete_GenreAssignedToMovie_KeepsTheMovie()
    {
        var genreId = 0;
        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var genre = TestData.NewGenre("Drama");
            var movie = TestData.NewMovie(category.Id, "Survives genre deletion");
            movie.Genres.Add(genre);
            db.Movies.Add(movie);
            await db.SaveChangesAsync();
            genreId = genre.Id;
        });
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.DeleteAsync($"/api/genre/delete/{genreId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var movies = await Factory.QueryAsync(db => db.Movies.CountAsync());
        Assert.Equal(1, movies);
    }

    [Fact]
    public async Task GetAll_ReturnsAllGenres()
    {
        await SeedGenreAsync("Drama");
        await SeedGenreAsync("Comedy");
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var genres = await user.GetFromJsonAsync<List<GenreItem>>("/api/genre/all");

        Assert.Equal(new[] { "Comedy", "Drama" }, genres!.Select(g => g.Name).OrderBy(n => n));
    }

    [Fact]
    public async Task GetAll_WithoutToken_DoesNotReturnServerError()
    {
        var response = await Client.GetAsync("/api/genre/all");

        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    private async Task<int> SeedGenreAsync(string name = "Drama")
    {
        var id = 0;
        await Factory.SeedAsync(async db =>
        {
            var genre = TestData.NewGenre(name);
            db.Genres.Add(genre);
            await db.SaveChangesAsync();
            id = genre.Id;
        });
        return id;
    }
}