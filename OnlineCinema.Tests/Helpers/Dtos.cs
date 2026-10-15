namespace OnlineCinema.Tests.Helpers
{
    public record MovieItem(Guid Id, string Title);
    public record RecentViewItem(Guid MovieId, string Title);
    public record TopGenreItem(string Name, int Count);
    public record UserStatsItem(
        Guid UserId,
        int TotalViews,
        int TotalLikes,
        int TotalDislikes,
        int TotalComments,
        int TotalFavorites,
        int MoviesThisWeek,
        int MoviesThisMonth,
        int CurrentStreak,
        List<RecentViewItem> RecentlyViewed,
        List<TopGenreItem> TopGenres);
    public record CategoryItem(int Id, string Name);
    public record GenreItem(int Id, string Name);
    public record MovieDetails(
        Guid Id,
        string Title,
        string Country,
        decimal Review,
        int RecommendedAge,
        string? VideoUrl,
        CategoryItem Category,
        List<GenreItem> Genres);
    public record MovieLikes(Guid Id, int Likes, int Dislikes);
    public record MessageItem(string Message);
    public record ActorItem(Guid Id, string FullName, string Biography, string? ImageUrl);
}
