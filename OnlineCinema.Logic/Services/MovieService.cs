using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
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
