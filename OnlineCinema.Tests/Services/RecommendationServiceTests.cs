using Moq;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.Services;
using Xunit;

namespace OnlineCinema.Tests.Services;

public class RecommendationServiceTests
{
    private static Movie CreateMovie(Guid id, DateOnly dateRealise, IEnumerable<Genre>? genres = null, IEnumerable<Actor>? actors = null)
    {
        return new Movie
        {
            Id = id,
            Title = $"Movie-{id}",
            Category = new Category { Id = 1, Name = "TestCategory" },
            Description = "test description",
            Country = "UA",
            DateRealise = dateRealise,
            Genres = (genres ?? Enumerable.Empty<Genre>()).ToList(),
            Actors = (actors ?? Enumerable.Empty<Actor>()).ToList()
        };
    }

    private static UserActivity CreateActivity(Guid userId, string entityId, EntityType entityType, ActionType actionType, double weight)
    {
        return new UserActivity
        {
            UserId = userId,
            EntityId = entityId,
            EntityType = entityType,
            ActionType = actionType,
            Weight = weight
        };
    }

    [Fact]
    public async Task GetNewReleasesAsync_ReturnsMoviesWithinWindow_OrderedByDateDescending()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var recentMovie1 = CreateMovie(Guid.NewGuid(), today.AddMonths(-1));
        var recentMovie2 = CreateMovie(Guid.NewGuid(), today.AddMonths(-3));
        var oldMovie = CreateMovie(Guid.NewGuid(), today.AddMonths(-12));

        var movieRepoMock = new Mock<IMovieRepository>();
        movieRepoMock.Setup(r => r.GetAllMoviesAsync())
            .ReturnsAsync(new List<Movie> { oldMovie, recentMovie2, recentMovie1 });

        var service = new RecommendationService(new Mock<IUserActivityRepository>().Object, movieRepoMock.Object);

        var result = await service.GetNewReleasesAsync(6);

        Assert.Equal(2, result.Count);
        Assert.Equal(recentMovie1.Id, result[0].Id);
        Assert.Equal(recentMovie2.Id, result[1].Id);
    }

    [Fact]
    public async Task GetNewReleasesAsync_WhenNoMoviesWithinWindow_ReturnsEmptyList()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var oldMovie = CreateMovie(Guid.NewGuid(), today.AddMonths(-24));

        var movieRepoMock = new Mock<IMovieRepository>();
        movieRepoMock.Setup(r => r.GetAllMoviesAsync()).ReturnsAsync(new List<Movie> { oldMovie });

        var service = new RecommendationService(new Mock<IUserActivityRepository>().Object, movieRepoMock.Object);

        var result = await service.GetNewReleasesAsync(6);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPopularMoviesAsync_RanksByLikeAndFavoriteCount_ExcludingViews()
    {
        var userId = Guid.NewGuid();
        var popularMovie = CreateMovie(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow));
        var viewedOnlyMovie = CreateMovie(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow));

        var activities = new List<UserActivity>
        {
            CreateActivity(userId, popularMovie.Id.ToString(), EntityType.Movie, ActionType.Like, 2.0),
            CreateActivity(userId, popularMovie.Id.ToString(), EntityType.Movie, ActionType.Favorite, 3.0),
            CreateActivity(userId, viewedOnlyMovie.Id.ToString(), EntityType.Movie, ActionType.View, 1.0),
            CreateActivity(userId, "actor-1", EntityType.Actor, ActionType.Like, 2.0)
        };

        var activityRepoMock = new Mock<IUserActivityRepository>();
        activityRepoMock.Setup(r => r.GetMovieActivitiesAsync()).ReturnsAsync(activities);

        var movieRepoMock = new Mock<IMovieRepository>();
        movieRepoMock.Setup(r => r.GetMovieByIdAsync(popularMovie.Id)).ReturnsAsync(popularMovie);

        var service = new RecommendationService(activityRepoMock.Object, movieRepoMock.Object);

        var result = await service.GetPopularMoviesAsync(10);

        Assert.Single(result);
        Assert.Equal(popularMovie.Id, result[0].Id);
        movieRepoMock.Verify(r => r.GetMovieByIdAsync(viewedOnlyMovie.Id), Times.Never);
    }

    [Fact]
    public async Task GetPopularMoviesAsync_WhenMovieNotFound_SkipsGracefully()
    {
        var userId = Guid.NewGuid();
        var missingMovieId = Guid.NewGuid();

        var activities = new List<UserActivity>
        {
            CreateActivity(userId, missingMovieId.ToString(), EntityType.Movie, ActionType.Like, 2.0)
        };

        var activityRepoMock = new Mock<IUserActivityRepository>();
        activityRepoMock.Setup(r => r.GetMovieActivitiesAsync()).ReturnsAsync(activities);

        var movieRepoMock = new Mock<IMovieRepository>();
        movieRepoMock.Setup(r => r.GetMovieByIdAsync(missingMovieId)).ReturnsAsync((Movie?)null);

        var service = new RecommendationService(activityRepoMock.Object, movieRepoMock.Object);

        var result = await service.GetPopularMoviesAsync(10);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRecommendationsForUserAsync_WhenUserHasNoActivities_ReturnsEmptyWithoutQueryingMovies()
    {
        var userId = Guid.NewGuid();

        var activityRepoMock = new Mock<IUserActivityRepository>();
        activityRepoMock.Setup(r => r.GetUserActivitiesByUserId(userId)).ReturnsAsync(new List<UserActivity>());

        var movieRepoMock = new Mock<IMovieRepository>();

        var service = new RecommendationService(activityRepoMock.Object, movieRepoMock.Object);

        var result = await service.GetRecommendationsForUserAsync(userId);

        Assert.Empty(result);
        movieRepoMock.Verify(r => r.GetMovieByIdAsync(It.IsAny<Guid>()), Times.Never);
        movieRepoMock.Verify(r => r.GetMoviesByGenreIdAsync(It.IsAny<int>()), Times.Never);
        movieRepoMock.Verify(r => r.GetMoviesByActorAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetRecommendationsForUserAsync_ExcludesMoviesUserAlreadyInteractedWith()
    {
        var userId = Guid.NewGuid();
        var genre = new Genre { Id = 1, Name = "Drama" };

        var likedMovie = CreateMovie(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), genres: new[] { genre });
        var newMovie = CreateMovie(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1), genres: new[] { genre });

        var activities = new List<UserActivity>
        {
            CreateActivity(userId, likedMovie.Id.ToString(), EntityType.Movie, ActionType.Like, 2.0)
        };

        var activityRepoMock = new Mock<IUserActivityRepository>();
        activityRepoMock.Setup(r => r.GetUserActivitiesByUserId(userId)).ReturnsAsync(activities);

        var movieRepoMock = new Mock<IMovieRepository>();
        movieRepoMock.Setup(r => r.GetMovieByIdAsync(likedMovie.Id)).ReturnsAsync(likedMovie);
        movieRepoMock.Setup(r => r.GetMoviesByGenreIdAsync(genre.Id))
            .ReturnsAsync(new List<Movie> { likedMovie, newMovie });

        var service = new RecommendationService(activityRepoMock.Object, movieRepoMock.Object);

        var result = await service.GetRecommendationsForUserAsync(userId);

        Assert.Single(result);
        Assert.Equal(newMovie.Id, result[0].Id);
    }

    [Fact]
    public async Task GetRecommendationsForUserAsync_DeduplicatesMoviesFoundViaBothGenreAndActor()
    {
        var userId = Guid.NewGuid();
        var genre = new Genre { Id = 1, Name = "Drama" };
        var actor = new Actor { Id = Guid.NewGuid(), FullName = "Test Actor" };

        var likedMovie = CreateMovie(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), genres: new[] { genre }, actors: new[] { actor });
        var sharedMovie = CreateMovie(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2), genres: new[] { genre }, actors: new[] { actor });

        var activities = new List<UserActivity>
        {
            CreateActivity(userId, likedMovie.Id.ToString(), EntityType.Movie, ActionType.Favorite, 3.0)
        };

        var activityRepoMock = new Mock<IUserActivityRepository>();
        activityRepoMock.Setup(r => r.GetUserActivitiesByUserId(userId)).ReturnsAsync(activities);

        var movieRepoMock = new Mock<IMovieRepository>();
        movieRepoMock.Setup(r => r.GetMovieByIdAsync(likedMovie.Id)).ReturnsAsync(likedMovie);
        movieRepoMock.Setup(r => r.GetMoviesByGenreIdAsync(genre.Id)).ReturnsAsync(new List<Movie> { likedMovie, sharedMovie });
        movieRepoMock.Setup(r => r.GetMoviesByActorAsync(actor.Id)).ReturnsAsync(new List<Movie> { likedMovie, sharedMovie });

        var service = new RecommendationService(activityRepoMock.Object, movieRepoMock.Object);

        var result = await service.GetRecommendationsForUserAsync(userId);

        Assert.Single(result);
        Assert.Equal(sharedMovie.Id, result[0].Id);
    }
}
