using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineCinema.Tests.Fakes;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;

namespace OnlineCinema.Tests.Actors;

[Collection("Api")]
public class ActorTests : IntegrationTestBase
{
    private const string SomeId = "00000000-0000-0000-0000-000000000001";

    public ActorTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Theory]
    [InlineData("POST", "/api/actor/add")]
    [InlineData("PUT", "/api/actor/edit/" + SomeId)]
    [InlineData("DELETE", "/api/actor/delete/" + SomeId)]
    [InlineData("PATCH", "/api/actor/upload-actor-image/" + SomeId)]
    public async Task AdminEndpoints_WithoutToken_ReturnUnauthorized(string method, string url)
    {
        var response = await Client.SendAsync(new HttpRequestMessage(new HttpMethod(method), url));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("POST", "/api/actor/add")]
    [InlineData("PUT", "/api/actor/edit/" + SomeId)]
    [InlineData("DELETE", "/api/actor/delete/" + SomeId)]
    [InlineData("PATCH", "/api/actor/upload-actor-image/" + SomeId)]
    public async Task AdminEndpoints_AsRegularUser_ReturnForbidden(string method, string url)
    {
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await user.SendAsync(new HttpRequestMessage(new HttpMethod(method), url));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Add_AsAdmin_ReturnsCreatedActor()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PostAsJsonAsync("/api/actor/add",
            new { fullName = "Brad Pitt", biography = "American actor" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actor = await response.Content.ReadFromJsonAsync<ActorItem>();
        Assert.Equal("Brad Pitt", actor!.FullName);
        Assert.Equal("American actor", actor.Biography);
        Assert.NotEqual(Guid.Empty, actor.Id);
    }

    [Fact]
    public async Task Add_WithEmptyName_ReturnsBadRequest()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PostAsJsonAsync("/api/actor/add", new { fullName = "", biography = "bio" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Edit_AsAdmin_UpdatesNameAndBiography()
    {
        var id = await SeedActorAsync("Old name", "Old bio");
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PutAsJsonAsync($"/api/actor/edit/{id}",
            new { id, fullName = "New name", biography = "New bio", imageUrl = (string?)null });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actor = await response.Content.ReadFromJsonAsync<ActorItem>();
        Assert.Equal("New name", actor!.FullName);
        Assert.Equal("New bio", actor.Biography);
    }

    [Fact]
    public async Task Edit_UnknownActor_ReturnsNotFound()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);
        var id = Guid.NewGuid();

        var response = await admin.PutAsJsonAsync($"/api/actor/edit/{id}",
            new { id, fullName = "Ghost", biography = "none", imageUrl = (string?)null });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AsAdmin_RemovesActorAndRequestsFileCleanup()
    {
        var id = await SeedActorAsync();
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.DeleteAsync($"/api/actor/delete/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var afterDelete = await admin.GetAsync($"/api/actor/{id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);

        var fake = Factory.Services.GetRequiredService<FakeActorUploadService>();
        Assert.Contains(id, fake.DeletedActorIds);
    }

    [Fact]
    public async Task Delete_UnknownActor_ReturnsNotFoundWithoutFileCleanup()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);
        var id = Guid.NewGuid();

        var response = await admin.DeleteAsync($"/api/actor/delete/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var fake = Factory.Services.GetRequiredService<FakeActorUploadService>();
        Assert.DoesNotContain(id, fake.DeletedActorIds);
    }

    [Fact]
    public async Task Delete_ActorWithMovies_KeepsTheMovie()
    {
        var actorId = Guid.Empty;
        await Factory.SeedAsync(async db =>
        {
            var category = TestData.NewCategory();
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var actor = TestData.NewActor("Leaves the cast");
            var movie = TestData.NewMovie(category.Id, "Survives actor deletion");
            movie.Actors.Add(actor);
            db.Movies.Add(movie);
            actorId = actor.Id;
            await db.SaveChangesAsync();
        });
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.DeleteAsync($"/api/actor/delete/{actorId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var movies = await Factory.QueryAsync(db => db.Movies.CountAsync());
        Assert.Equal(1, movies);
    }

    [Fact]
    public async Task GetById_ExistingActor_ReturnsActor_AndUnknownReturnsNotFound()
    {
        var id = await SeedActorAsync("Brad Pitt", "bio");
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var actor = await user.GetFromJsonAsync<ActorItem>($"/api/actor/{id}");
        var missing = await user.GetAsync($"/api/actor/{Guid.NewGuid()}");

        Assert.Equal("Brad Pitt", actor!.FullName);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task GetById_WithoutToken_DoesNotReturnServerError()
    {
        var id = await SeedActorAsync();

        var response = await Client.GetAsync($"/api/actor/{id}");

        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsAllActors()
    {
        await SeedActorAsync("Brad Pitt");
        await SeedActorAsync("Angelina Jolie");
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var actors = await user.GetFromJsonAsync<List<ActorItem>>("/api/actor/all");

        Assert.Equal(new[] { "Angelina Jolie", "Brad Pitt" }, actors!.Select(a => a.FullName).OrderBy(n => n));
    }

    [Fact]
    public async Task GetByFullName_ReturnsMatches_AndEmptyListWhenNothingMatches()
    {
        await SeedActorAsync("Brad Pitt");
        await SeedActorAsync("Angelina Jolie");
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var found = await user.GetFromJsonAsync<List<ActorItem>>("/api/actor/get-by-fullname?fullName=Pitt");
        var none = await user.GetFromJsonAsync<List<ActorItem>>("/api/actor/get-by-fullname?fullName=Nobody");

        Assert.Equal(new[] { "Brad Pitt" }, found!.Select(a => a.FullName));
        Assert.Empty(none!);
    }

    private async Task<Guid> SeedActorAsync(string name = "Test Actor", string biography = "bio")
    {
        var id = Guid.Empty;
        await Factory.SeedAsync(async db =>
        {
            var actor = TestData.NewActor(name);
            actor.Biography = biography;
            db.Actors.Add(actor);
            id = actor.Id;
            await db.SaveChangesAsync();
        });
        return id;
    }
}