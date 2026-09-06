using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface IMovieRepository
    {
        Task<Movie> AddMovieAsync(Movie movie, List<int>? genreIds, List<Guid>? actorIds, List<int>? audioTrackIds, List<int>? platformIds);
        Task<bool> DeleteMovieAsync(Guid movieId);
        Task<Movie> EditMovieAsync(Movie movie, List<int>? genreIds, List<Guid>? actorIds, List<int>? audioTrackIds, List<int>? platformIds);
        Task<List<Movie>> GetAllMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(Guid movieId);
        Task<Movie?> GetMovieByTitleAsync(string title);
        Task<List<Movie>> GetMoviesByAudioTrackAsync(string language);
        Task<List<Movie>> GetMoviesByCategoryAsync(int categoryId);
        Task<List<Movie>> GetMoviesByCountryAsync(string country);
        Task<List<Movie>> GetMoviesByGenreIdAsync(int genreId);
        Task<List<Movie>> GetMoviesByRatingAsync(decimal minRating);
        Task<List<Movie>> GetMoviesByYearAsync(int year);
        Task<List<Movie>> GetMoviesByActorAsync(Guid actorId);
    }
}
