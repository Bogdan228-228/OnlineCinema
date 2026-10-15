namespace OnlineCinema.Logic.Dto
{
    public record UserStatsDto(
        Guid UserId,
        int TotalViews,
        int TotalLikes,
        int TotalDislikes,
        int TotalComments,
        int TotalFavorites,
        int MoviesThisWeek,
        int MoviesThisMonth,
        int CurrentStreak,
        List<RecentViewDto> RecentlyViewed,
        List<TopGenreDto> TopGenres,
        List<TopActorDto> TopActors
    );

    public record RecentViewDto(
        Guid MovieId,
        string Title,
        string? PosterUrl,
        DateTime ViewedAt
    );

    public record TopGenreDto(string Name, int Count);
    public record TopActorDto(Guid Id, string FullName, string? ImageUrl, int Count);
}