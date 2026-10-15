using System.Net;
using System.Net.Http.Json;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;

namespace OnlineCinema.Tests.Movies;

[Collection("Api")]
public class MovieLikesTests : IntegrationTestBase
{
    public MovieLikesTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Like_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Patch(Client, "like", Guid.NewGuid());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Like_UnknownMovie_ReturnsNotFound()
    {
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var response = await Patch(user, "like", Guid.NewGuid());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Like_IncrementsLikes()
    {
        var movie = await Factory.SeedMovieAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        var result = await LikeAsync(user, "like", movie.Id);

        Assert.Equal(1, result.Likes);
        Assert.Equal(0, result.Dislikes);
    }

    [Fact]
    public async Task Like_Twice_CountsOnlyOnce()
    {
        var movie = await Factory.SeedMovieAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        await LikeAsync(user, "like", movie.Id);
        var second = await LikeAsync(user, "like", movie.Id);

        Assert.Equal(1, second.Likes);
    }

    [Fact]
    public async Task Like_ByTwoUsers_CountsBoth()
    {
        var movie = await Factory.SeedMovieAsync();
        var (first, _) = await AuthHelper.CreateUserClientAsync(Factory);
        var (second, _) = await AuthHelper.CreateUserClientAsync(Factory);

        await LikeAsync(first, "like", movie.Id);
        var result = await LikeAsync(second, "like", movie.Id);

        Assert.Equal(2, result.Likes);
    }

    [Fact]
    public async Task Dislike_AfterLike_SwitchesReaction()
    {
        var movie = await Factory.SeedMovieAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        await LikeAsync(user, "like", movie.Id);
        var result = await LikeAsync(user, "dislike", movie.Id);

        Assert.Equal(0, result.Likes);
        Assert.Equal(1, result.Dislikes);
    }

    [Fact]
    public async Task RemoveLike_DecrementsLikes()
    {
        var movie = await Factory.SeedMovieAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        await LikeAsync(user, "like", movie.Id);
        var result = await LikeAsync(user, "remove-like", movie.Id);

        Assert.Equal(0, result.Likes);
    }

    [Fact]
    public async Task Like_AfterMovieWasCached_GetByIdReturnsUpdatedLikes()
    {
        var movie = await Factory.SeedMovieAsync();
        var (user, _) = await AuthHelper.CreateUserClientAsync(Factory);

        await user.GetAsync($"/api/movie/{movie.Id}");
        await LikeAsync(user, "like", movie.Id);

        var loaded = await user.GetFromJsonAsync<MovieLikes>($"/api/movie/{movie.Id}");

        Assert.Equal(1, loaded!.Likes);
    }

    private static Task<HttpResponseMessage> Patch(HttpClient client, string action, Guid movieId)
        => client.PatchAsync($"/api/movie/{action}/{movieId}", null);

    private static async Task<MovieLikes> LikeAsync(HttpClient client, string action, Guid movieId)
    {
        var response = await Patch(client, action, movieId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MovieLikes>())!;
    }
}