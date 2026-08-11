using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.API.DTOs;
using OnlineCinema.Domain.Abstractions.Services;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddGenre(CreateGenreRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = await _genreService.AddGenreAsync(request.Name);

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

            return Ok(new { message = "Actor deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
                return NotFound(new { message = "Genre not found" });

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

            var response = new GenreResponse(
                genre.Id,
                genre.Name
            );

            return Ok(response);
        }
    }
}
