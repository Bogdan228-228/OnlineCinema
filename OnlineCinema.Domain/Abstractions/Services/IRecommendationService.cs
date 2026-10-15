using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IRecommendationService
    {
        Task<List<Movie>> GetNewReleasesAsync(int months = 6);
        Task<List<Movie>> GetPopularMoviesAsync(int count = 10);
        Task<List<Movie>> GetRecommendationsForUserAsync(Guid userId);
    }
}
