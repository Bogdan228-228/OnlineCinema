using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Tests.Helpers
{
    public static class TestData
    {
        public static Category NewCategory(string name = "Фільм") => new() { Name = name };

        public static Movie NewMovie(int categoryId, string? title = null, DateOnly? released = null) => new()
        {
            Id = Guid.NewGuid(),
            Title = title ?? $"Movie {Guid.NewGuid().ToString("N")[..6]}",
            CategoryId = categoryId,
            DateRealise = released ?? DateOnly.FromDateTime(DateTime.UtcNow),
            Description = "test description",
            Country = "UA",
            PosterUrl = "poster.jpg",
            Review = 7.5m,
            RecommendedAge = 12,
            Duration = TimeSpan.FromMinutes(100)
        };

        public static User NewUser() => new()
        {
            Id = Guid.NewGuid(),
            UserName = $"{Guid.NewGuid():N}@test.com",
            Email = $"{Guid.NewGuid():N}@test.com"
        };

        public static UserActivity NewMovieActivity(Guid userId, Guid movieId, ActionType action, double weight = 1.0) => new()
        {
            UserId = userId,
            EntityId = movieId.ToString(),
            EntityType = EntityType.Movie,
            ActionType = action,
            Weight = weight
        };

        public static Genre NewGenre(string name) => new() { Name = name };

        public static Actor NewActor(string fullName) => new()
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Biography = "test biography"
        };
    }
}
