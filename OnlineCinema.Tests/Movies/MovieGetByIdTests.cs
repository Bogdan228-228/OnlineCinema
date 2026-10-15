using System.Net;
using System.Net.Http.Json;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;

namespace OnlineCinema.Tests.Movies;

[Collection("Api")]
public class MovieGetByIdTests : IntegrationTestBase
{
    public MovieGetByIdTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetById_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync($"/api/movie/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await user.GetAsync($"/api/movie/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ExistingMovie_ReturnsItsDetails()
    {
        var movieId = await SeedMovieAsync("Seeded movie");
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var movie = await user.GetFromJsonAsync<MovieDetails>($"/api/movie/{movieId}");

        Assert.Equal("Seeded movie", movie!.Title);
        Assert.Equal(movieId, movie.Id);
    }

    [Fact]
    public async Task GetById_MovieWithoutVideo_ReturnsNullVideoUrl()
    {
        var movieId = await SeedMovieAsync("No video yet");
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var movie = await user.GetFromJsonAsync<MovieDetails>($"/api/movie/{movieId}");

        Assert.Null(movie!.VideoUrl);
    }

    private async Task<Guid> SeedMovieAsync(string title)
    {
        var movieId = Guid.Empty;
        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var movie = TestData.NewMovie(category.Id, title);
            db.Movies.Add(movie);
            movieId = movie.Id;
            await db.SaveChangesAsync();
        });
        return movieId;
    }
}