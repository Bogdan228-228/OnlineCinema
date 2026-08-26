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
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IUserActivityService _userActivityService;

        public FavoriteController(IFavoriteService favoriteService, IUserActivityService userActivityService)
        {
            _favoriteService = favoriteService;
            _userActivityService = userActivityService;
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userId);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFavorite(CreateFavoriteRequest request)
        {
            var favorite = await _favoriteService.AddToFavoritesAsync(request.UserId, request.MovieId);
            if (favorite == null)
                return BadRequest("Favorite already exists or could not be created.");

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), favorite.Id.ToString(), EntityType.Favorite, ActionType.Post, weight: 3.0);

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

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), favorite.Id.ToString(), EntityType.Favorite, ActionType.Delete, weight: -3.0);

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

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), userId.ToString(), EntityType.Favorite, ActionType.Search);

            var response = favorites.Select(f => new FavoriteResponse(
                f.Id,
                f.User,
                f.Movie
            ));

            return Ok(response);
        }
    }
}
