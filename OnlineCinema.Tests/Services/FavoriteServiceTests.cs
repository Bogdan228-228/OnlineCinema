using OnlineCinema.Logic.DTOs.Favorites;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Services;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Services;

public class FavoriteServiceTests
{
    [Fact]
    public async Task AddAsync_WhenNotExists_AddsFavoriteAndReturnsResponse()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new FavoriteService(dbContext);
        var userId = Guid.NewGuid();

        var result = await service.AddAsync(userId, new AddFavoriteRequest { ContentId = "movie-1" });

        Assert.Equal("movie-1", result.ContentId);
        Assert.Single(dbContext.Favorites);
    }

    [Fact]
    public async Task AddAsync_WhenAlreadyExists_ThrowsConflictException()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new FavoriteService(dbContext);
        var userId = Guid.NewGuid();

        await service.AddAsync(userId, new AddFavoriteRequest { ContentId = "movie-1" });

        await Assert.ThrowsAsync<ConflictException>(
            () => service.AddAsync(userId, new AddFavoriteRequest { ContentId = "movie-1" }));

        Assert.Single(dbContext.Favorites);
    }

    [Fact]
    public async Task RemoveAsync_WhenNotFound_ThrowsNotFoundException()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new FavoriteService(dbContext);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.RemoveAsync(Guid.NewGuid(), "unknown-content"));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyRequestedUsersFavorites()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new FavoriteService(dbContext);
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        await service.AddAsync(userId, new AddFavoriteRequest { ContentId = "movie-1" });
        await service.AddAsync(userId, new AddFavoriteRequest { ContentId = "movie-2" });
        await service.AddAsync(otherUserId, new AddFavoriteRequest { ContentId = "movie-3" });

        var result = await service.GetAllAsync(userId);

        Assert.Equal(2, result.Count);
        Assert.All(result, f => Assert.Contains(f.ContentId, new[] { "movie-1", "movie-2" }));
    }

    [Fact]
    public async Task IsFavoriteAsync_ReturnsFalse_WhenNotAdded()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new FavoriteService(dbContext);

        var isFavorite = await service.IsFavoriteAsync(Guid.NewGuid(), "movie-1");

        Assert.False(isFavorite);
    }
}
