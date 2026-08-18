using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.API.DTOs;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Enums;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IUserActivityService _userActivityService;

        public MovieController(IMovieService movieService, IUserActivityService userActivityService)
        {
            _movieService = movieService;
            _userActivityService = userActivityService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddMovie(CreateMovieRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var movie = await _movieService.AddMovieAsync(
                request.Title, request.CategoryId, request.Review,
                request.RecommendedAge, request.DateRealise, request.Duration,
                request.Description, request.Country, request.ImgUrl,
                request.GenreIds, request.ActorIds, request.AudioTrackIds, request.PlatformIds
            );

            return Ok(MapMovie(movie));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("edit/{id}")]
        public async Task<IActionResult> EditMovie(Guid id, EditMovieRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var movie = await _movieService.EditMovieAsync(
                id, request?.Title, request?.CategoryId, request?.Review,
                request?.RecommendedAge, request?.DateRealise, request?.Duration,
                request?.Likes, request?.Dislikes, request?.Description, request?.Country,
                request?.ImgUrl, request?.GenreIds, request?.ActorIds, request?.AudioTrackIds, request?.PlatformIds
            );

            if (movie == null) return NotFound(new { message = "Movie not found" });
            return Ok(MapMovie(movie));
        }

        [Authorize]
        [HttpPatch("like/{id}")]
        public async Task<IActionResult> LikeMovie(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var movie = await _movieService.LikeMovieAsync(userId, id);
            if (movie == null) return NotFound(new { message = "Movie not found" });

            return Ok(movie);
        }

        [Authorize]
        [HttpPatch("remove-like/{id}")]
        public async Task<IActionResult> RemoveLike(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var movie = await _movieService.RemoveLikeAsync(userId, id);
            if (movie == null) return NotFound(new { message = "Movie not found" });

            await _userActivityService.DeleteActivityAsync(userId, movie.Id, ActionType.Like);
            return Ok(movie);
        }

        [Authorize]
        [HttpPatch("dislike/{id}")]
        public async Task<IActionResult> DislikeMovie(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var movie = await _movieService.DislikeMovieAsync(userId, id);
            if (movie == null) return NotFound(new { message = "Movie not found" });

            return Ok(movie);
        }

        [Authorize]
        [HttpPatch("remove-dislike/{id}")]
        public async Task<IActionResult> RemoveDislike(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var movie = await _movieService.RemoveDislikeAsync(userId, id);
            if (movie == null) return NotFound(new { message = "Movie not found" });

            await _userActivityService.DeleteActivityAsync(userId, movie.Id, ActionType.Dislike);
            return Ok(movie);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var result = await _movieService.DeleteMovieAsync(id);
            if (!result) return NotFound(new { message = "Movie not found" });
            return Ok(new { message = "Movie deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null) return NotFound(new { message = "Movie not found" });

            if (User?.Identity?.IsAuthenticated == true)
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

                await _userActivityService.AddActivityAsync(userId, movie.Id, ActionType.View);
            }

            return Ok(MapMovie(movie));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies.Select(MapMovie));
        }

        [HttpGet("by-title")]
        public async Task<IActionResult> GetMovieByTitle(string title)
        {
            var movie = await _movieService.GetMovieByTitleAsync(title);
            if (movie == null) return NotFound(new { message = "Movie not found" });
            return Ok(MapMovie(movie));
        }

        [HttpGet("by-audio")]
        public async Task<IActionResult> GetMoviesByAudioTrack(string language)
        {
            var movies = await _movieService.GetMoviesByAudioTrackAsync(language);
            return Ok(movies.Select(MapMovie));
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetMoviesByCategory(int categoryId)
        {
            var movies = await _movieService.GetMoviesByCategoryAsync(categoryId);
            return Ok(movies.Select(MapMovie));
        }

        [HttpGet("by-country")]
        public async Task<IActionResult> GetMoviesByCountry(string country)
        {
            var movies = await _movieService.GetMoviesByCountryAsync(country);
            return Ok(movies.Select(MapMovie));
        }

        [HttpGet("by-genre")]
        public async Task<IActionResult> GetMoviesByGenre(string genreName)
        {
            var movies = await _movieService.GetMoviesByGenreAsync(genreName);
            return Ok(movies.Select(MapMovie));
        }

        [HttpGet("by-rating")]
        public async Task<IActionResult> GetMoviesByRating(decimal minRating)
        {
            var movies = await _movieService.GetMoviesByRatingAsync(minRating);
            return Ok(movies.Select(MapMovie));
        }

        [HttpGet("by-year")]
        public async Task<IActionResult> GetMoviesByYear(int year)
        {
            var movies = await _movieService.GetMoviesByYearAsync(year);
            return Ok(movies.Select(MapMovie));
        }

        [HttpGet("by-actor/{id}")]
        public async Task<IActionResult> GetMoviesByActor(Guid id)
        {
            var movies = await _movieService.GetMoviesByActorAsync(id);
            if (movies == null || !movies.Any()) return NotFound(new { message = "Movie not found" });
            return Ok(movies.Select(MapMovie));
        }

        private MovieResponse MapMovie(Domain.Models.Movie movie)
        {
            return new MovieResponse(
                movie.Id,
                movie.Title,
                movie.Review,
                movie.RecommendedAge,
                movie.DateRealise,
                movie.Duration,
                movie.Description,
                movie.Country,
                movie.ImgUrl,
                movie.Likes,
                movie.Dislikes,
                new CategoryResponse(movie.Category.Id, movie.Category.Name),
                movie.Genres.Select(g => new GenreResponse(g.Id, g.Name)).ToList(),
                movie.Actors.Select(a => new ActorResponse(a.Id, a.FullName, a.Biography)).ToList(),
                movie.AudioTracks.Select(at => new AudioTrackResponse(at.Id, at.Language)).ToList(),
                movie.Platforms.Select(p => new PlatformResponse(p.Id, p.Name)).ToList()
            );
        }
    }
}
