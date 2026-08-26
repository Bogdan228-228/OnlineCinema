using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IUserActivityRepository _userActivityRepository;

        public MovieService(IMovieRepository movieRepository, IUserActivityRepository userActivityRepository)
        {
            _movieRepository = movieRepository;
            _userActivityRepository = userActivityRepository;
        }

        public async Task<Movie> AddMovieAsync(
            string title,
            int categoryId,
            decimal review,
            int recommendedAge,
            DateOnly dateRealise,
            TimeSpan duration,
            string description,
            string country,
            string imgUrl = "",
            List<int>? genreIds = null,
            List<Guid>? actorIds = null,
            List<int>? audioTrackIds = null,
            List<int>? platformIds = null)
        {
            var movie = new Movie
            {
                Id = Guid.NewGuid(),
                Title = title,
                CategoryId = categoryId,
                Review = review,
                RecommendedAge = recommendedAge,
                DateRealise = dateRealise,
                Duration = duration,
                Description = description,
                Country = country,
                ImgUrl = imgUrl,
                Likes = 0,
                Dislikes = 0
            };

            return await _movieRepository.AddMovieAsync(movie, genreIds, actorIds, audioTrackIds, platformIds);
        }


        public async Task<Movie?> EditMovieAsync(
            Guid id,
            string? title,
            int? categoryId,
            decimal? review,
            int? recommendedAge,
            DateOnly? dateRealise,
            TimeSpan? duration,
            int? likes,
            int? dislikes,
            string? description,
            string? country,
            string? imgUrl = null,
            List<int>? genreIds = null,
            List<Guid>? actorIds = null,
            List<int>? audioTrackIds = null,
            List<int>? platformIds = null)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(id);
            if (movie == null)
                return null;

            if (title != null) movie.Title = title;
            if (categoryId.HasValue) movie.CategoryId = categoryId.Value;
            if (review.HasValue) movie.Review = review.Value;
            if (recommendedAge.HasValue) movie.RecommendedAge = recommendedAge.Value;
            if (dateRealise.HasValue) movie.DateRealise = dateRealise.Value;
            if (duration.HasValue) movie.Duration = duration.Value;
            if (likes.HasValue) movie.Likes = likes.Value;
            if (dislikes.HasValue) movie.Dislikes = dislikes.Value;
            if (description != null) movie.Description = description;
            if (country != null) movie.Country = country;
            if (imgUrl != null) movie.ImgUrl = imgUrl;

            return await _movieRepository.EditMovieAsync(movie, genreIds, actorIds, audioTrackIds, platformIds);
        }

        public async Task<Movie?> LikeMovieAsync(Guid userId, Guid movieId)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie == null) return null;

            var hasLike = await _userActivityRepository.Exists(userId, movieId.ToString(), EntityType.Movie, ActionType.Like);
            var hasDislike = await _userActivityRepository.Exists(userId, movieId.ToString(), EntityType.Movie, ActionType.Dislike);

            if (hasLike) return movie;
            if (hasDislike)
            {
                if (movie.Dislikes > 0) movie.Dislikes--;
                await _movieRepository.EditMovieAsync(movie, null, null, null, null);
                await _userActivityRepository.DeleteActivityAsync(userId, movieId.ToString(), EntityType.Movie, ActionType.Dislike);
            }

            movie.Likes++;
            await _movieRepository.EditMovieAsync(movie, null, null, null, null);

            await _userActivityRepository.AddUserActivity(new UserActivity
            {
                UserId = userId,
                EntityId = movieId.ToString(),
                EntityType = EntityType.Movie,
                ActionType = ActionType.Like,
                Timestamp = DateTime.UtcNow,
                Weight = 2.0
            });

            return movie;
        }

        public async Task<Movie?> RemoveLikeAsync(Guid userId, Guid movieId)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie == null) return null;

            var activity = (await _userActivityRepository.GetUserActivitiesByAction(userId, ActionType.Like))
                .FirstOrDefault(a => a.EntityId == movieId.ToString() && a.EntityType == EntityType.Movie);

            if (activity != null)
            {
                if (movie.Likes > 0) movie.Likes--;
                await _movieRepository.EditMovieAsync(movie, null, null, null, null);
                await _userActivityRepository.DeleteActivityAsync(userId, movieId.ToString(), EntityType.Movie, ActionType.Like);
            }

            return movie;
        }

        public async Task<Movie?> DislikeMovieAsync(Guid userId, Guid movieId)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie == null) return null;

            var hasLike = await _userActivityRepository.Exists(userId, movieId.ToString(), EntityType.Movie, ActionType.Like);
            var hasDislike = await _userActivityRepository.Exists(userId, movieId.ToString(), EntityType.Movie, ActionType.Dislike);

            if (hasDislike) return movie;
            if (hasLike)
            {
                if (movie.Likes > 0) movie.Likes--;
                await _movieRepository.EditMovieAsync(movie, null, null, null, null);
                await _userActivityRepository.DeleteActivityAsync(userId, movieId.ToString(), EntityType.Movie, ActionType.Like);
            }

            movie.Dislikes++;
            await _movieRepository.EditMovieAsync(movie, null, null, null, null);

            await _userActivityRepository.AddUserActivity(new UserActivity
            {
                UserId = userId,
                EntityId = movieId.ToString(),
                EntityType = EntityType.Movie,
                ActionType = ActionType.Dislike,
                Timestamp = DateTime.UtcNow,
                Weight = -1.0
            });

            return movie;
        }

        public async Task<Movie?> RemoveDislikeAsync(Guid userId, Guid movieId)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie == null) return null;

            var activity = (await _userActivityRepository.GetUserActivitiesByAction(userId, ActionType.Dislike))
                .FirstOrDefault(a => a.EntityId == movieId.ToString() && a.EntityType == EntityType.Movie);

            if (activity != null)
            {
                if (movie.Dislikes > 0) movie.Dislikes--;
                await _movieRepository.EditMovieAsync(movie, null, null, null, null);
                await _userActivityRepository.DeleteActivityAsync(userId, movieId.ToString(), EntityType.Movie, ActionType.Dislike);
            }

            return movie;
        }

        public async Task<bool> DeleteMovieAsync(Guid movieId)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie != null)
            {
                return await _movieRepository.DeleteMovieAsync(movie.Id);
            }
            return false;
        }

        public async Task<Movie?> GetMovieByIdAsync(Guid movieId)
        {
            return await _movieRepository.GetMovieByIdAsync(movieId);
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _movieRepository.GetAllMoviesAsync();
        }

        public async Task<Movie?> GetMovieByTitleAsync(string title)
        {
            return await _movieRepository.GetMovieByTitleAsync(title);
        }

        public async Task<List<Movie>> GetMoviesByActorAsync(Guid actorId)
        {
            return await _movieRepository.GetMoviesByActorAsync(actorId);
        }

        public async Task<List<Movie>> GetMoviesByCategoryAsync(int categoryId)
        {
            return await _movieRepository.GetMoviesByCategoryAsync(categoryId);
        }

        public async Task<List<Movie>> GetMoviesByGenreAsync(string genreName)
        {
            return await _movieRepository.GetMoviesByGenreAsync(genreName);
        }

        public async Task<List<Movie>> GetMoviesByRatingAsync(decimal minRating)
        {
            return await _movieRepository.GetMoviesByRatingAsync(minRating);
        }

        public async Task<List<Movie>> GetMoviesByYearAsync(int year)
        {
            return await _movieRepository.GetMoviesByYearAsync(year);
        }

        public async Task<List<Movie>> GetMoviesByAudioTrackAsync(string language)
        {
            return await _movieRepository.GetMoviesByAudioTrackAsync(language);
        }

        public async Task<List<Movie>> GetMoviesByCountryAsync(string country)
        {
            return await _movieRepository.GetMoviesByCountryAsync(country);
        }
    }
}
