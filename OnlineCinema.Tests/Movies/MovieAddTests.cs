using System.Net;
using System.Net.Http.Json;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Movies;

[Collection("Api")]
public class MovieAddTests : IntegrationTestBase
{
    public MovieAddTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Add_WithoutToken_ReturnsUnauthorized()
    {
        var categoryId = await SeedCategoryAsync();

        var response = await Client.PostAsJsonAsync("/api/movie/add", ValidMovie(categoryId));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Add_AsRegularUser_ReturnsForbidden()
    {
        var categoryId = await SeedCategoryAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await user.PostAsJsonAsync("/api/movie/add", ValidMovie(categoryId));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Add_AsAdmin_CreatesMovieThatCanBeRetrieved()
    {
        var categoryId = await SeedCategoryAsync();
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PostAsJsonAsync("/api/movie/add", ValidMovie(categoryId));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<MovieDetails>();

        var loaded = await admin.GetFromJsonAsync<MovieDetails>($"/api/movie/{created!.Id}");

        Assert.Equal("New movie", loaded!.Title);
        Assert.Equal("UA", loaded.Country);
        Assert.Equal(categoryId, loaded.Category.Id);
    }

    [Fact]
    public async Task Add_WithGenreIds_AttachesGenres()
    {
        var categoryId = await SeedCategoryAsync();
        var genreId = 0;
        await Factory.SeedAsync(async db =>
        {
            var genre = TestData.NewGenre("Drama");
            db.Genres.Add(genre);
            await db.SaveChangesAsync();
            genreId = genre.Id;
        });
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PostAsJsonAsync("/api/movie/add", ValidMovie(categoryId, new[] { genreId }));
        var created = await response.Content.ReadFromJsonAsync<MovieDetails>();

        Assert.Equal(new[] { "Drama" }, created!.Genres.Select(g => g.Name));
    }

    private async Task<int> SeedCategoryAsync()
    {
        var categoryId = 0;
        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            categoryId = category.Id;
        });
        return categoryId;
    }

    private static object ValidMovie(int categoryId, int[]? genreIds = null) => new
    {
        title = "New movie",
        categoryId,
        review = 4.5,
        recommendedAge = 12,
        dateRealise = "2026-01-15",
        duration = "01:40:00",
        description = "Test description",
        country = "UA",
        imgUrl = "https://example.com/poster.jpg",
        genreIds
    };
}