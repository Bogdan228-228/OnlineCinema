using OnlineCinema.Logic.DTOs.History;
using OnlineCinema.Logic.Services;
using OnlineCinema.Tests.Helpers;
using Xunit;

namespace OnlineCinema.Tests.Services;

public class HistoryServiceTests
{
    [Fact]
    public async Task UpsertProgressAsync_WhenProgressBelowThreshold_MarksNotCompleted()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new HistoryService(dbContext);
        var userId = Guid.NewGuid();

        var result = await service.UpsertProgressAsync(userId, new UpsertHistoryRequest
        {
            ContentId = "movie-1",
            ProgressSeconds = 100,
            DurationSeconds = 1000
        });

        Assert.False(result.Completed);
    }

    [Fact]
    public async Task UpsertProgressAsync_WhenProgressAtOrAboveNinetyPercent_MarksCompleted()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new HistoryService(dbContext);
        var userId = Guid.NewGuid();

        var result = await service.UpsertProgressAsync(userId, new UpsertHistoryRequest
        {
            ContentId = "movie-1",
            ProgressSeconds = 900,
            DurationSeconds = 1000
        });

        Assert.True(result.Completed);
    }

    [Fact]
    public async Task UpsertProgressAsync_WhenCalledTwice_UpdatesExistingRecordInsteadOfDuplicating()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new HistoryService(dbContext);
        var userId = Guid.NewGuid();

        await service.UpsertProgressAsync(userId, new UpsertHistoryRequest
        {
            ContentId = "movie-1",
            ProgressSeconds = 100,
            DurationSeconds = 1000
        });

        await service.UpsertProgressAsync(userId, new UpsertHistoryRequest
        {
            ContentId = "movie-1",
            ProgressSeconds = 500,
            DurationSeconds = 1000
        });

        Assert.Single(dbContext.History);
        Assert.Equal(500, dbContext.History.First().ProgressSeconds);
    }

    [Fact]
    public async Task UpsertProgressAsync_OnceCompleted_StaysCompletedEvenIfProgressResets()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new HistoryService(dbContext);
        var userId = Guid.NewGuid();

        await service.UpsertProgressAsync(userId, new UpsertHistoryRequest
        {
            ContentId = "movie-1",
            ProgressSeconds = 950,
            DurationSeconds = 1000
        });

        var result = await service.UpsertProgressAsync(userId, new UpsertHistoryRequest
        {
            ContentId = "movie-1",
            ProgressSeconds = 10,
            DurationSeconds = 1000
        });

        Assert.True(result.Completed);
    }

    [Fact]
    public async Task GetByContentAsync_WhenNoRecordExists_ReturnsNull()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new HistoryService(dbContext);

        var result = await service.GetByContentAsync(Guid.NewGuid(), "unknown-movie");

        Assert.Null(result);
    }

    [Fact]
    public async Task ClearAllAsync_RemovesAllHistoryForUserOnly()
    {
        var dbContext = TestDbContextFactory.Create();
        var service = new HistoryService(dbContext);
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        await service.UpsertProgressAsync(userId, new UpsertHistoryRequest { ContentId = "movie-1", ProgressSeconds = 10 });
        await service.UpsertProgressAsync(userId, new UpsertHistoryRequest { ContentId = "movie-2", ProgressSeconds = 20 });
        await service.UpsertProgressAsync(otherUserId, new UpsertHistoryRequest { ContentId = "movie-3", ProgressSeconds = 30 });

        await service.ClearAllAsync(userId);

        Assert.Empty(await service.GetAllAsync(userId));
        Assert.Single(await service.GetAllAsync(otherUserId));
    }
}
