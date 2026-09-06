using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IUserActivityRepository _userActivityRepository;
        private readonly IMovieRepository _movieRepository;

        public RecommendationService(IUserActivityRepository userActivityRepository, IMovieRepository movieRepository)
        {
            _userActivityRepository = userActivityRepository;
            _movieRepository = movieRepository;
        }

        public async Task<List<Movie>> GetRecommendationsForUserAsync(Guid userId)
        {
            var activities = await _userActivityRepository.GetUserActivitiesByUserId(userId);

            var movieActivities = activities
                .Where(a => a.EntityType == EntityType.Movie
                         && (a.ActionType == ActionType.Favorite
                          || a.ActionType == ActionType.Like
                          || a.ActionType == ActionType.View))
                .GroupBy(a => a.EntityId)
                .Select(g => new
                {
                    MovieId = g.Key,
                    TotalWeight = g.Sum(x => x.Weight)
                })
                .OrderByDescending(x => x.TotalWeight)
                .Take(10)
                .ToList();

            var likedMovies = new List<Movie>();
            foreach (var movieActivity in movieActivities)
            {
                var movie = await _movieRepository.GetMovieByIdAsync(Guid.Parse(movieActivity.MovieId));
                if (movie != null) likedMovies.Add(movie);
            }

            var topGenres = likedMovies
                .SelectMany(m => m.Genres)
                .GroupBy(g => g.Id)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var topActors = likedMovies
                .SelectMany(m => m.Actors)
                .GroupBy(a => a.Id)
                .OrderByDescending(a => a.Count())
                .Take(3)
                .Select(a => a.Key)
                .ToList();

            var recommendedMovies = new List<Movie>();
            foreach (var genreId in topGenres)
            {
                var movies = await _movieRepository.GetMoviesByGenreIdAsync(genreId);
                recommendedMovies.AddRange(movies);
            }
            foreach (var actorId in topActors)
            {
                var movies = await _movieRepository.GetMoviesByActorAsync(actorId);
                recommendedMovies.AddRange(movies);
            }

            return recommendedMovies
                .Where(m => !movieActivities.Select(ma => ma.MovieId).Contains(m.Id.ToString()))
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .OrderByDescending(m => m.DateRealise)
                .ToList();
        }

        public async Task<List<Movie>> GetNewReleasesAsync(int months = 6)
        {
            var cutoffDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-months);

            var movies = await _movieRepository.GetAllMoviesAsync();

            var newReleases = movies
                .Where(m => m.DateRealise >= cutoffDate)
                .OrderByDescending(m => m.DateRealise)
                .ToList();

            return newReleases;
        }

        public async Task<List<Movie>> GetPopularMoviesAsync(int count = 10)
        {
            var allActivities = await _userActivityRepository.GetMovieActivitiesAsync();

            var popularMovieIds = allActivities
                .Where(a => a.EntityType == EntityType.Movie
                         && (a.ActionType == ActionType.Favorite || a.ActionType == ActionType.Like))
                .GroupBy(a => a.EntityId)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.Key)
                .ToList();

            var movies = new List<Movie>();
            foreach (var id in popularMovieIds)
            {
                var movie = await _movieRepository.GetMovieByIdAsync(Guid.Parse(id));
                if (movie != null) movies.Add(movie);
            }

            return movies;
        }
    }
}
