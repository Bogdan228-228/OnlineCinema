using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface IMovieRepository
    {
        Task<Movie> AddMovieAsync(Movie movie);
        Task<Movie> DeleteMovieAsync(int movieId);
        Task<Movie> EditMovieAsync(Movie movie);
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
