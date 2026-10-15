using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using OnlineCinema.Tests.Fixtures;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Actors;

[Collection("Api")]
public class ActorImageTests : IntegrationTestBase
{
    public ActorImageTests(CinemaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task UploadImage_AsAdmin_SetsImageUrl()
    {
        var id = await SeedActorAsync();
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PatchAsync($"/api/actor/upload-actor-image/{id}", ImageContent("photo.jpg"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actor = await response.Content.ReadFromJsonAsync<ActorItem>();
        Assert.Contains("photo.jpg", actor!.ImageUrl);
    }

    [Fact]
    public async Task UploadImage_EmptyFile_ReturnsBadRequest()
    {
        var id = await SeedActorAsync();
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PatchAsync($"/api/actor/upload-actor-image/{id}", ImageContent("empty.jpg", empty: true));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UploadImage_UnknownActor_ReturnsNotFound()
    {
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);

        var response = await admin.PatchAsync($"/api/actor/upload-actor-image/{Guid.NewGuid()}", ImageContent("photo.jpg"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_AfterImageUpload_ReturnsImageUrl()
    {
        var id = await SeedActorAsync();
        var admin = await AuthHelper.CreateAdminClientAsync(Factory);
        (await admin.PatchAsync($"/api/actor/upload-actor-image/{id}", ImageContent("photo.jpg")))
            .EnsureSuccessStatusCode();

        var actor = await admin.GetFromJsonAsync<ActorItem>($"/api/actor/{id}");

        Assert.Contains("photo.jpg", actor!.ImageUrl);
    }

    private static MultipartFormDataContent ImageContent(string fileName, bool empty = false)
    {
        var file = new ByteArrayContent(empty ? Array.Empty<byte>() : new byte[] { 1, 2, 3 });
        file.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        return new MultipartFormDataContent { { file, "file", fileName } };
    }

    private async Task<Guid> SeedActorAsync()
    {
        var id = Guid.Empty;
        await Factory.SeedAsync(async db =>
        {
            var actor = TestData.NewActor("Photo Actor");
            db.Actors.Add(actor);
            id = actor.Id;
            await db.SaveChangesAsync();
        });
        return id;
    }
}