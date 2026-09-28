using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Movie>>> GetUserRecommendations(Guid userId)
        {
            var movies = await _recommendationService.GetRecommendationsForUserAsync(userId);
            return Ok(movies);
        }

        [HttpGet("new")]
        public async Task<ActionResult<List<Movie>>> GetNewReleases([FromQuery] int months = 6)
        {
            var movies = await _recommendationService.GetNewReleasesAsync(months);
            return Ok(movies);
        }

        [HttpGet("popular")]
        public async Task<ActionResult<List<Movie>>> GetPopularMovies([FromQuery] int count = 10)
        {
            var movies = await _recommendationService.GetPopularMoviesAsync(count);
            return Ok(movies);
        }
    }
}
