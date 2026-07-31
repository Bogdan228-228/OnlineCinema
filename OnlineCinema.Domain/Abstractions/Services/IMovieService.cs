using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IMovieService
    {
        Task<Movie> AddMovieAsync(string title, int categoryId, decimal review, int recommendedAge, DateOnly dateRealise, TimeSpan duration, string description, string country, string imgUrl = "");
        Task<bool> DeleteMovieAsync(Guid movieId);
        Task<Movie?> EditMovieAsync(Guid id, string title, int categoryId, decimal review, int recommendedAge, DateOnly dateRealise, TimeSpan duration, string description, string country, string imgUrl = "");
        Task<List<Movie>> GetAllMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(Guid movieId);
        Task<Movie?> GetMovieByTitleAsync(string title);
        Task<List<Movie>> GetMoviesByAudioTrackAsync(string language);
        Task<List<Movie>> GetMoviesByCategoryAsync(int categoryId);
        Task<List<Movie>> GetMoviesByCountryAsync(string country);
        Task<List<Movie>> GetMoviesByGenreAsync(string genreName);
        Task<List<Movie>> GetMoviesByRatingAsync(decimal minRating);
        Task<List<Movie>> GetMoviesByYearAsync(int year);
        Task<Movie?> GetMovieWithActorsAsync(Guid movieId);
        Task<List<Movie>> SearchMoviesByTitleAsync(string title);
    }
}
