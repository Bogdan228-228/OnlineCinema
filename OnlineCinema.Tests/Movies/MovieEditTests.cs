using System.Net;
using System.Net.Http.Json;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Movies;

[Collection("Api")]
public class MovieEditTests : IntegrationTestBase
{
    public MovieEditTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Edit_AsRegularUser_ReturnsForbidden()
    {
        var movie = await Factory.SeedMovieAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await user.PatchAsync($"/api/movie/edit/{movie.Id}",
            JsonContent.Create(new { id = movie.Id, title = "Hacked" }));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Edit_AsAdmin_UpdatesOnlyProvidedFields()
    {
        var movie = await Factory.SeedMovieAsync("Original", country: "UA");
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PatchAsync($"/api/movie/edit/{movie.Id}",
            JsonContent.Create(new { id = movie.Id, title = "Updated" }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<MovieDetails>();
        Assert.Equal("Updated", updated!.Title);
        Assert.Equal("UA", updated.Country);
    }

    [Fact]
    public async Task Edit_UnknownMovie_ReturnsNotFound()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);
        var id = Guid.NewGuid();

        var response = await admin.PatchAsync($"/api/movie/edit/{id}",
            JsonContent.Create(new { id, title = "Ghost" }));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Edit_AfterMovieWasCached_GetByIdReturnsUpdatedTitle()
    {
        var movie = await Factory.SeedMovieAsync("Original");
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        await admin.GetAsync($"/api/movie/{movie.Id}");

        var edit = await admin.PatchAsync($"/api/movie/edit/{movie.Id}",
            JsonContent.Create(new { id = movie.Id, title = "Updated" }));
        edit.EnsureSuccessStatusCode();

        var loaded = await admin.GetFromJsonAsync<MovieDetails>($"/api/movie/{movie.Id}");

        Assert.Equal("Updated", loaded!.Title);
    }
}