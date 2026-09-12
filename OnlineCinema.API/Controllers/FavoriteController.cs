using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Logic.DTOs.Favorites;
using OnlineCinema.Logic.Interfaces;
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
            var userIdString = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdString))
                throw new InvalidOperationException("User ID claim is missing");

            return Guid.Parse(userIdString);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFavorite(AddFavoriteRequest request)
        {
            var favorite = await _favoriteService.AddAsync(GetCurrentUserId(), request);
            if (favorite == null)
                return BadRequest("Favorite already exists or could not be created.");

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), favorite.Id.ToString(), EntityType.Favorite, ActionType.Post, weight: 3.0);

            var response = new DTOs.FavoriteResponse(favorite.Id);

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveFavorite(Guid id)
        {
            var favorite = await _favoriteService.RemoveAsync(GetCurrentUserId(), id);
            if (favorite == null)
                return NotFound("Favorite not found.");

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), favorite.Id.ToString(), EntityType.Favorite, ActionType.Delete, weight: -3.0);

            var response = new FavoriteResponse 
            { 
                Id = favorite.Id,
                ContentId = favorite.ContentId,
                CreatedAt = favorite.CreatedAt 
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("favorites/{userId:guid}")]
        public async Task<IActionResult> GetFavoritesByUser(Guid userId)
        {
            var favorites = await _favoriteService.GetAllAsync(userId);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), userId.ToString(), EntityType.Favorite, ActionType.Search);

            var response = favorites.Select(f => new FavoriteResponse
            {
                Id = f.Id,
                ContentId = f.ContentId,
                CreatedAt = f.CreatedAt
            });

            return Ok(response);
        }
    }
}
