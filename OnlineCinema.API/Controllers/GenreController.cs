using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.API.DTOs;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Enums;
using System.Security.Claims;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;
        private readonly IUserActivityService _userActivityService;

        public GenreController(IGenreService genreService, IUserActivityService userActivityService)
        {
            _genreService = genreService;
            _userActivityService = userActivityService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdString))
                throw new InvalidOperationException("User ID claim is missing");

            return Guid.Parse(userIdString);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddGenre(CreateGenreRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = await _genreService.AddGenreAsync(request.Name);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), genre.Id.ToString(), EntityType.Genre, ActionType.Post);

            var response = new GenreResponse(
                genre.Id,
                genre.Name
            );

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditGenre(int id, EditGenreRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = await _genreService.EditGenreAsync(id, request.Name);
            if (genre == null)
                return NotFound(new { message = "Genre not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), genre.Id.ToString(), EntityType.Genre, ActionType.Put);

            var response = new GenreResponse(
                genre.Id,
                genre.Name
            );

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteActor(int id)
        {
            var result = await _genreService.DeleteGenreAsync(id);
            if (!result)
                return NotFound(new { message = "Actor not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), id.ToString(), EntityType.Genre, ActionType.Delete);

            return Ok(new { message = "Actor deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
                return NotFound(new { message = "Genre not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), genre.Id.ToString(), EntityType.Genre, ActionType.View);

            var response = new GenreResponse(
                genre.Id,
                genre.Name
            );

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenresAsync();

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.Genre, ActionType.Search);

            var response = genres.Select(g => new GenreResponse(
                g.Id,
                g.Name
            ));

            return Ok(response);
        }

        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetGenreByName(string name)
        {
            var genre = await _genreService.GetGenreByNameAsync(name);
            if (genre == null)
                return NotFound(new { message = "Genre not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.Genre, ActionType.Search);

            var response = new GenreResponse(
                genre.Id,
                genre.Name
            );

            return Ok(response);
        }
    }
}
