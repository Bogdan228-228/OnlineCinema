using System.Net;
using System.Net.Http.Json;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;

namespace OnlineCinema.Tests.Categories;

[Collection("Api")]
public class CategoryTests : IntegrationTestBase
{
    public CategoryTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Theory]
    [InlineData("POST", "/api/category/add")]
    [InlineData("PUT", "/api/category/edit/1")]
    [InlineData("DELETE", "/api/category/delete/1")]
    public async Task AdminEndpoints_WithoutToken_ReturnUnauthorized(string method, string url)
    {
        var response = await Client.SendAsync(new HttpRequestMessage(new HttpMethod(method), url));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("POST", "/api/category/add")]
    [InlineData("PUT", "/api/category/edit/1")]
    [InlineData("DELETE", "/api/category/delete/1")]
    public async Task AdminEndpoints_AsRegularUser_ReturnForbidden(string method, string url)
    {
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await user.SendAsync(new HttpRequestMessage(new HttpMethod(method), url));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Add_AsAdmin_ReturnsCreatedCategory()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PostAsJsonAsync("/api/category/add", new { name = "Series" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var category = await response.Content.ReadFromJsonAsync<CategoryItem>();
        Assert.Equal("Series", category!.Name);
        Assert.True(category.Id > 0);
    }

    [Fact]
    public async Task Add_WithEmptyName_ReturnsBadRequest()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PostAsJsonAsync("/api/category/add", new { name = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Edit_AsAdmin_RenamesCategory()
    {
        var id = await SeedCategoryAsync("Old name");
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PutAsJsonAsync($"/api/category/edit/{id}", new { id, name = "Renamed" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var category = await response.Content.ReadFromJsonAsync<CategoryItem>();
        Assert.Equal("Renamed", category!.Name);
    }

    [Fact]
    public async Task Edit_UnknownCategory_ReturnsNotFound()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PutAsJsonAsync("/api/category/edit/99999", new { id = 99999, name = "Ghost" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AsAdmin_RemovesCategory()
    {
        var id = await SeedCategoryAsync();
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.DeleteAsync($"/api/category/delete/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var afterDelete = await admin.GetAsync($"/api/category/{id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownCategory_ReturnsNotFound()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.DeleteAsync("/api/category/delete/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_CategoryWithMovies_ReturnsConflict()
    {
        var movie = await Factory.SeedMovieAsync();
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.DeleteAsync($"/api/category/delete/{movie.CategoryId}");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsAllCategories()
    {
        await SeedCategoryAsync("Drama");
        await SeedCategoryAsync("Comedy");
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var categories = await user.GetFromJsonAsync<List<CategoryItem>>("/api/category/all");

        Assert.Equal(new[] { "Comedy", "Drama" }, categories!.Select(c => c.Name).OrderBy(n => n));
    }

    [Fact]
    public async Task GetByName_ReturnsMatchingCategory_AndNotFoundForUnknown()
    {
        await SeedCategoryAsync("Drama");
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var found = await user.GetFromJsonAsync<CategoryItem>("/api/category/get-by-name?name=Drama");
        var missing = await user.GetAsync("/api/category/get-by-name?name=Nothing");

        Assert.Equal("Drama", found!.Name);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithoutToken_DoesNotReturnServerError()
    {
        var response = await Client.GetAsync("/api/category/all");

        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    private async Task<int> SeedCategoryAsync(string name = "Drama")
    {
        var id = 0;
        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory(name);
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            id = category.Id;
        });
        return id;
    }
}