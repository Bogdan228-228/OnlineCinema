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
        var favId = Guid.NewGuid().ToString();

        var result = await service.AddAsync(userId, new AddFavoriteRequest { ContentId = favId });

        Assert.Equal(favId, result.ContentId);
        Assert.Single(dbContext.Favorites);
    }

    [Fact]
    public async Task AddAsync_WhenAlreadyExists_ThrowsConflictException()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new FavoriteService(dbContext);
        var userId = Guid.NewGuid();
        var favId = Guid.NewGuid().ToString();

        await service.AddAsync(userId, new AddFavoriteRequest { ContentId = favId });

        await Assert.ThrowsAsync<ConflictException>(
            () => service.AddAsync(userId, new AddFavoriteRequest { ContentId = favId }));

        Assert.Single(dbContext.Favorites);
    }

    [Fact]
    public async Task RemoveAsync_WhenNotFound_ThrowsNotFoundException()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new FavoriteService(dbContext);
        var favId = Guid.NewGuid().ToString();

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.RemoveAsync(Guid.NewGuid(), favId));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyRequestedUsersFavorites()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new FavoriteService(dbContext);
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var favId1 = Guid.NewGuid().ToString();
        var favId2 = Guid.NewGuid().ToString();
        var favId3 = Guid.NewGuid().ToString();

        await service.AddAsync(userId, new AddFavoriteRequest { ContentId = favId1 });
        await service.AddAsync(userId, new AddFavoriteRequest { ContentId = favId2 });
        await service.AddAsync(otherUserId, new AddFavoriteRequest { ContentId = favId3 });

        var result = await service.GetAllAsync(userId);

        Assert.Equal(2, result.Count);
        Assert.All(result, f => Assert.Contains(f.ContentId, new[] { favId1, favId2 }));
    }

    [Fact]
    public async Task IsFavoriteAsync_ReturnsFalse_WhenNotAdded()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new FavoriteService(dbContext);
        var favId = Guid.NewGuid().ToString();

        var isFavorite = await service.IsFavoriteAsync(Guid.NewGuid(), favId);

        Assert.False(isFavorite);
    }
}
