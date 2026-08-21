using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.API.DTOs;
using OnlineCinema.Domain.Abstractions.Services;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFavorite(CreateFavoriteRequest request)
        {
            var favorite = await _favoriteService.AddToFavoritesAsync(request.UserId, request.MovieId);
            if (favorite == null)
                return BadRequest("Favorite already exists or could not be created.");

            var response = new FavoriteResponse(
                favorite.Id,
                favorite.User,
                favorite.Movie
            );

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveFavorite(Guid id)
        {
            var favorite = await _favoriteService.RemoveFromFavoritesAsync(id);
            if (favorite == null)
                return NotFound("Favorite not found.");

            var response = new FavoriteResponse(
                favorite.Id,
                favorite.User,
                favorite.Movie
            );

            return Ok(response);
        }

        [Authorize]
        [HttpGet("favorites/{userId:guid}")]
        public async Task<IActionResult> GetFavoritesByUser(Guid userId)
        {
            var favorites = await _favoriteService.GetFavoritesByUserIdAsync(userId);

            var response = favorites.Select(f => new FavoriteResponse(
                f.Id,
                f.User,
                f.Movie
            ));

            return Ok(response);
        }
    }
}
