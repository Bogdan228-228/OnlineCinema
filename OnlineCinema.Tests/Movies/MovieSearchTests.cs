using System.Net;
using System.Net.Http.Json;
using OnlineCinema.Domain.Models;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Movies;

[Collection("Api")]
public class MovieSearchTests : IntegrationTestBase
{
    public MovieSearchTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Theory]
    [InlineData("/api/movie/all")]
    [InlineData("/api/movie/by-title?title=x")]
    [InlineData("/api/movie/by-audio?language=x")]
    [InlineData("/api/movie/by-category/1")]
    [InlineData("/api/movie/by-country?country=UA")]
    [InlineData("/api/movie/by-genre/1")]
    [InlineData("/api/movie/by-rating?minRating=5")]
    [InlineData("/api/movie/by-year?year=2000")]
    [InlineData("/api/movie/by-actor/00000000-0000-0000-0000-000000000001")]
    public async Task Search_WithoutToken_DoesNotReturnServerError(string url)
    {
        var response = await Client.GetAsync(url);

        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task All_ReturnsEveryMovie()
    {
        await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var movies = await user.GetFromJsonAsync<List<MovieItem>>("/api/movie/all");

        Assert.Equal(new[] { "Chernobyl", "Funny Movie", "The Matrix" }, Titles(movies));
    }

    [Fact]
    public async Task ByCategory_ReturnsOnlyMoviesOfThatCategory()
    {
        var catalog = await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var movies = await user.GetFromJsonAsync<List<MovieItem>>($"/api/movie/by-category/{catalog.FilmCategoryId}");

        Assert.Equal(new[] { "Funny Movie", "The Matrix" }, Titles(movies));
    }

    [Fact]
    public async Task ByGenre_ReturnsOnlyMoviesOfThatGenre()
    {
        var catalog = await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var movies = await user.GetFromJsonAsync<List<MovieItem>>($"/api/movie/by-genre/{catalog.DramaId}");

        Assert.Equal(new[] { "Chernobyl", "The Matrix" }, Titles(movies));
    }

    [Fact]
    public async Task ByYear_ReturnsMoviesReleasedInThatYear()
    {
        await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var movies = await user.GetFromJsonAsync<List<MovieItem>>("/api/movie/by-year?year=1999");

        Assert.Equal(new[] { "The Matrix" }, Titles(movies));
    }

    [Fact]
    public async Task ByRating_ReturnsMoviesWithRatingAtLeastMinimum()
    {
        await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var top = await user.GetFromJsonAsync<List<MovieItem>>("/api/movie/by-rating?minRating=9");
        var good = await user.GetFromJsonAsync<List<MovieItem>>("/api/movie/by-rating?minRating=8");

        Assert.Equal(new[] { "Chernobyl" }, Titles(top));
        Assert.Equal(new[] { "Chernobyl", "The Matrix" }, Titles(good));
    }

    [Fact]
    public async Task ByCountry_ReturnsMoviesFromThatCountry()
    {
        await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var movies = await user.GetFromJsonAsync<List<MovieItem>>("/api/movie/by-country?country=UA");

        Assert.Equal(new[] { "Funny Movie" }, Titles(movies));
    }

    [Fact]
    public async Task ByAudio_ReturnsMoviesWithThatAudioTrack()
    {
        await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var movies = await user.GetFromJsonAsync<List<MovieItem>>("/api/movie/by-audio?language=Ukrainian");

        Assert.Equal(new[] { "Funny Movie" }, Titles(movies));
    }

    [Fact]
    public async Task ByActor_ReturnsMoviesWithThatActor()
    {
        var catalog = await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var movies = await user.GetFromJsonAsync<List<MovieItem>>($"/api/movie/by-actor/{catalog.ActorId}");

        Assert.Equal(new[] { "The Matrix" }, Titles(movies));
    }

    [Fact]
    public async Task ByActor_ActorWithoutMovies_ReturnsEmptyList()
    {
        await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await user.GetAsync($"/api/movie/by-actor/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var movies = await response.Content.ReadFromJsonAsync<List<MovieItem>>();
        Assert.Empty(movies!);
    }

    [Fact]
    public async Task ByTitle_ExactTitle_ReturnsMovie()
    {
        await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var movie = await user.GetFromJsonAsync<MovieItem>("/api/movie/by-title?title=Chernobyl");

        Assert.Equal("Chernobyl", movie!.Title);
    }

    [Fact]
    public async Task ByTitle_IgnoresLetterCase()
    {
        await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await user.GetAsync("/api/movie/by-title?title=matrix");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var movie = await response.Content.ReadFromJsonAsync<MovieItem>();
        Assert.Equal("The Matrix", movie!.Title);
    }

    [Fact]
    public async Task ByTitle_UnknownTitle_ReturnsNotFound()
    {
        await SeedCatalogAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await user.GetAsync("/api/movie/by-title?title=Nothing");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static string[] Titles(List<MovieItem>? movies)
        => movies!.Select(m => m.Title).OrderBy(t => t).ToArray();

    private sealed record Catalog(int FilmCategoryId, int SeriesCategoryId, int DramaId, Guid ActorId);

    private async Task<Catalog> SeedCatalogAsync()
    {
        Catalog? catalog = null;
        await Factory.SeedAsync(async db =>
        {
            var film = TestData.NewCategory("Film");
            var series = TestData.NewCategory("Series");
            db.Categories.AddRange(film, series);
            await db.SaveChangesAsync();

            var drama = TestData.NewGenre("Drama");
            var comedy = TestData.NewGenre("Comedy");
            var actor = TestData.NewActor("Keanu Reeves");
            var english = new AudioTrack { Language = "English" };
            var ukrainian = new AudioTrack { Language = "Ukrainian" };

            var matrix = TestData.NewMovie(film.Id, "The Matrix", new DateOnly(1999, 3, 31));
            matrix.Country = "USA";
            matrix.Review = 8.7m;
            matrix.Genres.Add(drama);
            matrix.Actors.Add(actor);
            matrix.AudioTracks.Add(english);

            var chernobyl = TestData.NewMovie(series.Id, "Chernobyl", new DateOnly(2019, 5, 6));
            chernobyl.Country = "UK";
            chernobyl.Review = 9.4m;
            chernobyl.Genres.Add(drama);
            chernobyl.AudioTracks.Add(english);

            var funny = TestData.NewMovie(film.Id, "Funny Movie", new DateOnly(2024, 1, 1));
            funny.Country = "UA";
            funny.Review = 6.0m;
            funny.Genres.Add(comedy);
            funny.AudioTracks.Add(ukrainian);

            db.Movies.AddRange(matrix, chernobyl, funny);
            await db.SaveChangesAsync();

            catalog = new Catalog(film.Id, series.Id, drama.Id, actor.Id);
        });
        return catalog!;
    }
}